using Shared.Data.Wrapper;
using Auth.Model.Models.Account;

namespace Account.API.Service.Interface
{
    public interface IAccountSettingsService
    {
        Task<IResponse<AccountSettingsResponse>> GetAccountSettingsAsync(int userId);
        Task<IResponse<AccountSettingsResponse>> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request);
    }
}
