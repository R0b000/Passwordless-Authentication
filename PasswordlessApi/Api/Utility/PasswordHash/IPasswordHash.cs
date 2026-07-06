namespace PasswordlessApi.Api.Utility.PasswordHash
{
    public interface IPasswordHash
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);  
    }
}