using API.Shared.Models.Entities;

namespace API.Shared.Service.Interface.Auth
{
    public interface IUserCredentialService
    {
        Task<List<UserCredential>> GetUserCredentialsAsync(int userId);
        Task<bool> HasCredentialsAsync(int userId);
    }
}