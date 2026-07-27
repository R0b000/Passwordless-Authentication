using Shared.Data.Wrapper;
using Auth.Model.Models.Account;

namespace Auth.API.Service.Interface.Auth
{
    public interface IAccountSettingsService
    {
        Task<IResponse<AccountSettingsResponse>> GetAccountSettingsAsync(int userId);
        Task<IResponse<AccountSettingsResponse>> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request);
    }
}
