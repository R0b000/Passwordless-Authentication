using PasswordlessApi.Api.Models.Entities;

namespace PasswordlessApi.Api.Service.Interface.Auth
{
    public interface IUserCredentialService
    {
        Task<List<UserCredential>> GetUserCredentialsAsync(int userId);
        Task<bool> HasCredentialsAsync(int userId);
    }
}