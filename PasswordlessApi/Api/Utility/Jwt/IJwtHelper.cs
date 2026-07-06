namespace PasswordlessApi.Api.Utility.Jwt
{
    public interface IJwtHelper
    {
        string GenerateToken(int userId, string username);
    }
}
