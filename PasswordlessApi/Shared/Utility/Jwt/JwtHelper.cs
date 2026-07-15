using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Shared.Utility.Jwt
{
    public class JwtHelper : IJwtHelper
    {
        private readonly string _secretKey;
        public string Issuer { get; }
        public string Audience { get; }
        private readonly int _expiryMinutes;
        private readonly int _refreshTokenExpiryDays;

        public JwtHelper(IConfiguration configuration)
        {
            var configuredSecret = configuration["JwtSettings:SecretKey"];
            var environmentSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            _secretKey = configuredSecret ?? environmentSecret ?? throw new InvalidOperationException("JWT signing secret is not configured.");
            Issuer = configuration["JwtSettings:Issuer"] ?? "PasswordlessApi";
            Audience = configuration["JwtSettings:Audience"] ?? "PasswordlessApiUsers";
            _expiryMinutes = int.TryParse(configuration["JwtSettings:ExpiryMinutes"], out var parsed) ? parsed : 60;
            _refreshTokenExpiryDays = int.TryParse(configuration["JwtSettings:RefreshTokenExpiryDays"], out var refreshParsed) ? refreshParsed : 7;

            var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
            if (keyBytes.Length < 16)
                throw new InvalidOperationException("JWT Secret Key must be at least 16 characters long.");
        }

        public string GenerateToken(int userId, string username)
        {
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username)
                }),
                Issuer = Issuer,
                Audience = Audience,
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GetSigningKey()
        {
            return _secretKey;
        }

        public int GetRefreshTokenExpiryDays()
        {
            return _refreshTokenExpiryDays;
        }
    }
}
