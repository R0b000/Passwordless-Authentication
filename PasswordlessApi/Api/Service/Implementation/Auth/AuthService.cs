using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;


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
        private static string ProcedureName = "sp_Users";

        public AuthService(IGenericRepository<UserIdResult> userIdRepository, IGenericRepository<User> userRepository, IPasswordHash passwordHash, IJwtHelper jwtHelper, IFido2Service fido2Service, IDapperRepository dapperRepository)
        {
            _userIdRepository = userIdRepository;
            _userRepository = userRepository;
            _passwordHash = passwordHash;
            _jwtHelper = jwtHelper;
            _fido2Service = fido2Service;
            _dapperRepository = dapperRepository;
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

            return new AuthResponse
            {
                UserId = userIdResult.Data.UserId,
                Username = request.Username,
                Email = request.Email,
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
            var userResult = await _userRepository.QuerySingleAsync(
                ProcedureName,
                new { AuthType = "Login", UserId = userId });
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

            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
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
    }
}
