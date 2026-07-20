using Shared.Core.Models.Entities;
using Shared.Core.Wrapper;

namespace Auth.API.Service.Interface.Auth
{
    public interface IUserCredentialService
    {
        Task<IResponse<List<UserCredential>>> GetUserCredentialsAsync(int userId);
        Task<IResponse<bool>> HasCredentialsAsync(int userId);
    }
}
