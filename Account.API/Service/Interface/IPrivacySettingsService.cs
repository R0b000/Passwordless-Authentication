using Shared.Data.Wrapper;
using Auth.Model.Models.Account;

namespace Account.API.Service.Interface
{
    public interface IPrivacySettingsService
    {
        Task<IResponse<PrivacySettingsResponse>> GetPrivacySettingsAsync(int userId);
        Task<IResponse<PrivacySettingsResponse>> UpdatePrivacySettingsAsync(int userId, UpdatePrivacyRequest request);
    }
}
