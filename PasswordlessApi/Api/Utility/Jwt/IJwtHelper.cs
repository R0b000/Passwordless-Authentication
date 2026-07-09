namespace PasswordlessApi.Api.Utility.Jwt
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username);
        string GenerateRefreshToken();
        string GetSigningKey();
        string Issuer { get; }
        string Audience { get; }
        int GetRefreshTokenExpiryDays();
    }
}
