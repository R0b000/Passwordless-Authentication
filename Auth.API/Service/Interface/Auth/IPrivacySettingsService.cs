using Shared.Data.Wrapper;
using Auth.Model.Models.Account;

namespace Auth.API.Service.Interface.Auth
{
    public interface IPrivacySettingsService
    {
        Task<IResponse<PrivacySettingsResponse>> GetPrivacySettingsAsync(int userId);
        Task<IResponse<PrivacySettingsResponse>> UpdatePrivacySettingsAsync(int userId, UpdatePrivacyRequest request);
    }
}
