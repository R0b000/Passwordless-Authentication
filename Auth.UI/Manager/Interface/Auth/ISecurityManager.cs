using Shared.Data.Wrapper;
using Auth.Model.Models.Security;
using Auth.Model.Models.Account;

namespace Auth.UI.Manager.Interface.Auth
{
    public interface ISecurityManager
    {
        Task<IResponse<SecuritySettingsResponse>> GetSecurityAsync();
        Task<IResponse<SecuritySettingsResponse>> UpdateSecurityAsync(SecuritySettingsRequest settings);
        Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<IResponse<SecuritySettingsResponse>> EnableTwoFactorAsync();
        Task<IResponse<SecuritySettingsResponse>> DisableTwoFactorAsync();
        Task<IResponse<List<SessionInfo>>> GetSessionsAsync();
        Task<IResponse<bool>> RevokeSessionAsync(string id);
        Task<IResponse<bool>> RevokeAllSessionsAsync(bool includingCurrent);
        Task<IResponse<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query);
        Task<IResponse<bool>> VerifyDeviceAsync(VerifyDeviceRequest request);
    }
}




