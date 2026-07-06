using System.Linq;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;
using System.Security.Cryptography;
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

            var userIdResult = await _userIdRepository.QuerySingleAsync(
                "sp_Users",
                new { AuthType = "Register", Username = request.Username, PasswordHash = passwordHash });

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
                Message = "Registered successfully"
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var userIdResult = await _userIdRepository.QuerySingleAsync(
                "sp_Users",
                new { AuthType = "Login", Username = request.Username });

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null || userIdResult.Data.UserId <= 0)
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var user = await _userRepository.QuerySingleAsync(
                "sp_Users",
                new { AuthType = "Login", UserId = userIdResult.Data.UserId });

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
                    Message = "FIDO2 verification required",
                    RequiresFido2 = true
                };
            }

            var token = _jwtHelper.GenerateToken(user.Data.Id, user.Data.Username);

            return new AuthResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username,
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
            var credentials = await _dapperRepository.QueryAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = userId });

            return credentials.ToList();
        }

        private async Task<bool> HasFido2CredentialsAsync(int userId)
        {
            var credentials = await _dapperRepository.QueryAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = userId });

            return credentials != null && credentials.Any();
        }
    }
}
