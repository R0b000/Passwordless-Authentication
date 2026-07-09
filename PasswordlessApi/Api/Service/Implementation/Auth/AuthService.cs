using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHash _passwordHash;
        private readonly IJwtHelper _jwtHelper;
        private readonly IFido2Service _fido2Service;
        private const string ProcedureName = "sp_Users";

        public AuthService(IAuthRepository authRepository, IPasswordHash passwordHash, IJwtHelper jwtHelper, IFido2Service fido2Service)
        {
            _authRepository = authRepository;
            _passwordHash = passwordHash;
            _jwtHelper = jwtHelper;
            _fido2Service = fido2Service;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var passwordHash = _passwordHash.HashPassword(request.Password);

            var param = new
            {
                AuthType = "Register",
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            var userIdResult = await _authRepository.QuerySingleAsync<UserIdResult>(ProcedureName, param);

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null)
            {
                return new AuthResponse
                {
                    Message = "Registration failed"
                };
            }

            var token = _jwtHelper.GenerateToken(userIdResult.Data.UserId, request.Username);
            await CreateRefreshTokenAsync(userIdResult.Data.UserId);

            return new AuthResponse
            {
                UserId = userIdResult.Data.UserId,
                Username = request.Username,
                Email = request.Email,
                Token = token,
                Message = "Registered successfully"
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var param = new
            {
                AuthType = "Login",
                Username = request.Username
            };

            var userIdResult = await _authRepository.QuerySingleAsync<UserIdResult>(
                ProcedureName,
                param
            );

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null || userIdResult.Data.UserId <= 0)
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var param_1 = new
            {
                AuthType = "Login",
                UserId = userIdResult.Data.UserId
            };

            var user = await _authRepository.QuerySingleAsync<User>(ProcedureName, param_1);

            if (user == null || !user.Succeeded || user.Data == null || string.IsNullOrEmpty(user.Data.PasswordHash))
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var isValid = _passwordHash.VerifyPassword(request.Password, user.Data.PasswordHash);

            if (!isValid)
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var hasFido2Credentials = await HasFido2CredentialsAsync(user.Data.Id);

            if (hasFido2Credentials)
            {
                return new AuthResponse
                {
                    UserId = user.Data.Id,
                    Username = user.Data.Username,
                    Email = user.Data.Email,
                    Message = "FIDO2 verification required",
                    RequiresFido2 = true
                };
            }

            var token = _jwtHelper.GenerateToken(user.Data.Id, user.Data.Username);
            await CreateRefreshTokenAsync(user.Data.Id);

            return new AuthResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username,
                Email = user.Data.Email,
                Token = token,
                Message = "Login successful",
                RequiresFido2 = false
            };
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = "Login", UserId = userId });
            return userResult.Succeeded ? userResult.Data : null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = "Login", Email = email });
            return userResult.Succeeded ? userResult.Data : null;
        }

        public async Task<OtpResponse> RequestOtpAsync(OtpRequest request)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = "Login", UserId = request.UserId });

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new OtpResponse { Success = false, Message = "User not found" };
            }

            var user = userResult.Data;
            if (string.IsNullOrEmpty(user.Email))
            {
                return new OtpResponse { Success = false, Message = "User does not have an email configured" };
            }

            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "EmailOtp",
                    FIDOOperation = "CreateOtp",
                    UserId = user.Id,
                    Otp = otp,
                    ExpiresAt = expiresAt
                });

            return new OtpResponse
            {
                Success = true,
                Message = $"OTP sent to {user.Email} (Demo OTP: {otp})",
                Otp = otp
            };
        }

        public async Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request)
        {
            var now = DateTime.UtcNow;
            var param = new
            {
                AuthType = "Login",
                UserId = request.UserId
            };

            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                param);

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new AuthResponse { Message = "User not found" };
            }

            var user = userResult.Data;
            var param_1 = new
            {
                AuthType = "EmailOtp",
                FIDOOperation = "ConsumeOtp",
                UserId = request.UserId,
                Otp = request.Otp,
                Now = now
            };

            var isConsumed = await _authRepository.QueryFirstAsync<bool>(
                ProcedureName,
                param_1);

            if (isConsumed != true)
            {
                return new AuthResponse { Message = "Invalid or expired OTP" };
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Username);
            await CreateRefreshTokenAsync(user.Id);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Message = "Login successful",
                RequiresFido2 = false
            };
        }

        public async Task<List<UserCredential>> GetUserCredentialsAsync(int userId)
        {
            var param = new
            {
                AuthType = "FIDO",
                FIDOOperation = "GetCredentialsByUserId",
                UserId = userId
            };

            var credentials = await _authRepository.QueryAsync<UserCredential>(
                ProcedureName,
                param);

            return credentials.ToList();
        }

        public async Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            return await _fido2Service.RequestAttestationOptionsAsync(request.UserId, request.Username);
        }

        public async Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            return await _fido2Service.RegisterCredentialAsync(request);
        }

        public async Task<Fido2ChallengeResponse> CreateFido2ChallengeAsync(Fido2ChallengeRequest request)
        {
            return await _fido2Service.CreateChallengeAsync(request.UserId);
        }

        public async Task<Fido2VerifyResponse> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            return await _fido2Service.VerifyAssertionAsync(request);
        }

        private async Task<bool> HasFido2CredentialsAsync(int userId)
        {
            var param = new
            {
                AuthType = "FIDO",
                FIDOOperation = "GetCredentialsByUserId",
                UserId = userId
            };

            var credentials = await _authRepository.QueryAsync<UserCredential>(
                ProcedureName,
                param);

            return credentials != null && credentials.Any();
        }

        private async Task CreateRefreshTokenAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var rawToken = _jwtHelper.GenerateRefreshToken();
            var tokenHash = _passwordHash.HashPassword(rawToken);
            var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "CreateRefreshToken",
                    UserId = userId,
                    TokenHash = tokenHash,
                    ExpiresAt = now.AddDays(refreshExpiryDays),
                    Now = now
                });
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal? principal = null;

            try
            {
                principal = tokenHandler.ValidateToken(
                    request.AccessToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtHelper.GetSigningKey())),
                        ValidateIssuer = true,
                        ValidIssuer = _jwtHelper.Issuer,
                        ValidateAudience = true,
                        ValidAudience = _jwtHelper.Audience,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero
                    },
                    out _);
            }
            catch
            {
                return new AuthResponse { Message = "Invalid access token" };
            }

            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return new AuthResponse { Message = "Invalid access token claims" };
            }

            var incomingTokenHash = _passwordHash.HashPassword(request.RefreshToken);

            var storedRefreshToken = await _authRepository.QuerySingleAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "GetRefreshToken",
                    TokenHash = incomingTokenHash
                });

            if (storedRefreshToken == null || !storedRefreshToken.Succeeded || storedRefreshToken.Data == null)
            {
                return new AuthResponse { Message = "Invalid refresh token" };
            }

            var refreshToken = storedRefreshToken.Data;

            if (refreshToken.UserId != userId)
            {
                return new AuthResponse { Message = "Refresh token does not belong to this user" };
            }

            if (refreshToken.IsRevoked)
            {
                return new AuthResponse { Message = "Refresh token has been revoked" };
            }

            if (refreshToken.ExpiresAt < now)
            {
                await _authRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = "RefreshToken",
                        FIDOOperation = "RevokeRefreshToken",
                        TokenHash = incomingTokenHash,
                        Now = now
                    });

                return new AuthResponse { Message = "Refresh token expired" };
            }

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "RevokeRefreshToken",
                    TokenHash = incomingTokenHash,
                    Now = now
                });

            var newAccessToken = _jwtHelper.GenerateToken(userId, usernameClaim ?? string.Empty);
            await CreateRefreshTokenAsync(userId);

            return new AuthResponse
            {
                UserId = userId,
                Username = usernameClaim ?? string.Empty,
                Token = newAccessToken,
                Message = "Token refreshed successfully"
            };
        }
    }
}