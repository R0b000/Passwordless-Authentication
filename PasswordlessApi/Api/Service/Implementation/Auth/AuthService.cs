using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Service.Interface.Auth;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<UserIdResult> _userIdRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHash _passwordHash;

        public AuthService(IGenericRepository<UserIdResult> userIdRepository, IGenericRepository<User> userRepository, IPasswordHash passwordHash)
        {
            _userIdRepository = userIdRepository;
            _userRepository = userRepository;
            _passwordHash = passwordHash;
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

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null || userIdResult.Data.UserId == Guid.Empty)
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

            return new AuthResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username,
                Message = "Login successful"
            };
        }
    }
}
