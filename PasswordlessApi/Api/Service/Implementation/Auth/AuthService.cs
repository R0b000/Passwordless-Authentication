using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PasswordlessApi.Api.Models;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<UserIdResult> _userIdRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHash _passwordHash;
        private readonly IConfiguration _configuration;

        public AuthService(IGenericRepository<UserIdResult> userIdRepository, IGenericRepository<User> userRepository, IPasswordHash passwordHash, IConfiguration configuration)
        {
            _userIdRepository = userIdRepository;
            _userRepository = userRepository;
            _passwordHash = passwordHash;
            _configuration = configuration;
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
                Message = "Registered successfully",
                Token = GenerateJwtToken(userIdResult.Data.UserId, request.Username)
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
                Message = "Login successful",
                Token = GenerateJwtToken(user.Data.Id, user.Data.Username)
            };
        }

        private string GenerateJwtToken(Guid userId, string username)
        {
            var secret = _configuration["JwtSettings:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrWhiteSpace(secret) || secret is "fake_jwt_token" or "fake_local_key")
            {
                throw new InvalidOperationException("JWT signing secret is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, username),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var issuer = _configuration["JwtSettings:Issuer"] ?? "PasswordlessApi";
            var audience = _configuration["JwtSettings:Audience"] ?? "PasswordlessApiUsers";
            var expiryMinutes = int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var parsedMinutes) ? parsedMinutes : 60;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
