using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Service.Interface.Rbac;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<UserIdResult> _userIdRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHash _passwordHash;
        private readonly IJwtHelper _jwtHelper;
        private readonly IFido2Service _fido2Service;
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private readonly GenerateSecureOtp _otpGenerator = new GenerateSecureOtp();
        private static string ProcedureName = "sp_Users";

        public AuthService(IGenericRepository<UserIdResult> userIdRepository, IGenericRepository<User> userRepository, IPasswordHash passwordHash, IJwtHelper jwtHelper, IFido2Service fido2Service, IDapperRepository dapperRepository, IUserRoleService userRoleService, IRoleService roleService)
        {
            _userIdRepository = userIdRepository;
            _userRepository = userRepository;
            _passwordHash = passwordHash;
            _jwtHelper = jwtHelper;
            _fido2Service = fido2Service;
            _dapperRepository = dapperRepository;
            _userRoleService = userRoleService;
            _roleService = roleService;
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

            var userIdResult = await _userIdRepository.QuerySingleAsync(ProcedureName, param);

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null)
            {
                return new AuthResponse
                {
                    Message = "Registration failed"
                };
            }

            var token = _jwtHelper.GenerateToken(userIdResult.Data.UserId, request.Username);
            var refreshToken = await CreateRefreshTokenAsync(userIdResult.Data.UserId);
            await AssignDefaultRoleIfMissingAsync(userIdResult.Data.UserId);

            var userWithRoles = await _userRoleService.GetUserWithRolesAndPermissionsAsync(userIdResult.Data.UserId);

            return new AuthResponse
            {
                UserId = userIdResult.Data.UserId,
                Username = request.Username,
                Email = request.Email,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Registered successfully",
                Role = userWithRoles?.Role,
                Permissions = userWithRoles?.Permissions ?? new List<string>()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var param = new
            {
                AuthType = "Login",
                Username = request.Username
            };

            var userIdResult = await _userIdRepository.QuerySingleAsync(
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
            
            var user = await _userRepository.QuerySingleAsync(ProcedureName, param_1);

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
            var refreshToken = await CreateRefreshTokenAsync(user.Data.Id);

            await AssignDefaultRoleIfMissingAsync(user.Data.Id);

            var userWithRoles = await _userRoleService.GetUserWithRolesAndPermissionsAsync(user.Data.Id);

            return new AuthResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username,
                Email = user.Data.Email,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Login successful",
                RequiresFido2 = false,
                Role = userWithRoles?.Role,
                Permissions = userWithRoles?.Permissions ?? new List<string>()
            };
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var userResult = await _userRepository.QuerySingleAsync(
                ProcedureName,
                new { AuthType = "Login", UserId = userId });
            return userResult.Succeeded ? userResult.Data : null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var userResult = await _userRepository.QuerySingleAsync(
                ProcedureName,
                new { AuthType = "Login", Email = email });
            return userResult.Succeeded ? userResult.Data : null;
        }

        public async Task<OtpResponse> RequestOtpAsync(OtpRequest request)
        {
            var userResult = await _userRepository.QuerySingleAsync(
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

            var otp = _otpGenerator.GenerateSecureOtpCode();
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            var dapperResult = await _dapperRepository.QuerySingleAsync<dynamic>(
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

            var userResult = await _userRepository.QuerySingleAsync(
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

            var isConsumed = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                param_1);

            if (!isConsumed)
            {
                return new AuthResponse { Message = "Invalid or expired OTP" };
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Username);
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken,
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

            var credentials = await _dapperRepository.QueryAsync<UserCredential>(
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

            var credentials = await _dapperRepository.QueryAsync<UserCredential>(
                ProcedureName,
                param);

            return credentials != null && credentials.Any();
        }

        private async Task AssignDefaultRoleIfMissingAsync(int userId)
        {
            var userRoles = await _userRoleService.GetUserRoleNamesAsync(userId);
            if (!userRoles.Any())
            {
                var user = await GetUserByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.Username))
                {
                    var roleName = user.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

                    var role = await _roleService.GetRoleByNameAsync(roleName);
                    if (role == null)
                    {
                        role = await _roleService.CreateRoleAsync(roleName, $"Default {roleName} role");
                    }

                    if (role != null)
                    {
                        await _userRoleService.AssignRoleToUserAsync(userId, role.Id);
                    }
                }
            }
        }

        private async Task<string> CreateRefreshTokenAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var refreshTokenHash = _passwordHash.HashPassword(refreshToken);
            var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            await _dapperRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "CreateRefreshToken",
                    UserId = userId,
                    Token = refreshTokenHash,
                    ExpiresAt = now.AddDays(refreshExpiryDays),
                    Now = now
                });

            return refreshToken;
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

            var storedRefreshToken = await _dapperRepository.QuerySingleAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "GetRefreshToken",
                    Token = request.RefreshToken
                });

            if (storedRefreshToken == null)
            {
                return new AuthResponse { Message = "Invalid refresh token" };
            }

            var refreshToken = storedRefreshToken;

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
                await _dapperRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = "RefreshToken",
                        FIDOOperation = "RevokeRefreshToken",
                        Token = request.RefreshToken,
                        Now = now
                    });

                return new AuthResponse { Message = "Refresh token expired" };
            }

            var newAccessToken = _jwtHelper.GenerateToken(userId, usernameClaim ?? string.Empty);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();
            var newRefreshTokenHash = _passwordHash.HashPassword(newRefreshToken);
            var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            await _dapperRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "CreateRefreshToken",
                    UserId = userId,
                    Token = newRefreshTokenHash,
                    ExpiresAt = now.AddDays(refreshExpiryDays),
                    Now = now
                });

            return new AuthResponse
            {
                UserId = userId,
                Username = usernameClaim ?? string.Empty,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Token refreshed successfully"
            };
        }
    }
}
