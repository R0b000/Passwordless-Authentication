using API.Shared.Models.Entities;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Auth
{
    public interface IUserCredentialService
    {
        Task<IResponse<List<UserCredential>>> GetUserCredentialsAsync(int userId);
        Task<IResponse<bool>> HasCredentialsAsync(int userId);
    }
}
