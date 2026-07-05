namespace PasswordlessApi.Security.PasswordHash
{
    public interface IPasswordHash
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);  
    }
}