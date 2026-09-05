using Auth.Model.Models.Entities;
using Shared.Data.Wrapper;

namespace Account.API.Service.Interface
{
    public interface IUserCredentialService
    {
        Task<IResponse<List<UserCredential>>> GetUserCredentialsAsync(int userId);
        Task<IResponse<bool>> HasCredentialsAsync(int userId);
    }
}


