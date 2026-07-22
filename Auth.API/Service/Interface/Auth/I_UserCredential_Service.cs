using Auth.Model.Models.Entities;
using Shared.Data.Wrapper;

namespace Auth.API.Service.Interface.Auth
{
    public interface IUserCredentialService
    {
        Task<IResponse<List<UserCredential>>> GetUserCredentialsAsync(int userId);
        Task<IResponse<bool>> HasCredentialsAsync(int userId);
    }
}


