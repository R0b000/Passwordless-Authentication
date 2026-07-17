using Shared.Wrapper;
using Auth.UI.Shared.Model.Security;

namespace UI.Shared.Manager.Interface.Auth
{
    public interface ISecurityManager
    {
        Task<IResponse<SecuritySettings>> GetSecurityAsync();
        Task<IResponse<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings);
        Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<IResponse<SecuritySettings>> EnableTwoFactorAsync();
        Task<IResponse<SecuritySettings>> DisableTwoFactorAsync();
        Task<IResponse<List<SessionInfo>>> GetSessionsAsync();
        Task<IResponse<bool>> RevokeSessionAsync(string id);
        Task<IResponse<bool>> RevokeAllSessionsAsync(bool includingCurrent);
        Task<IResponse<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query);
        Task<IResponse<bool>> VerifyDeviceAsync(VerifyDeviceRequest request);
    }
}
