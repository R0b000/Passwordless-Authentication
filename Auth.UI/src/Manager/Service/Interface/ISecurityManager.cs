using Auth.UI.src.Common;
using Auth.UI.src.Model.Security;

namespace Auth.UI.src.Manager.Service.Interface
{
    public interface ISecurityManager
    {
        Task<Response<SecuritySettings>> GetSecurityAsync();
        Task<Response<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings);
        Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<Response<SecuritySettings>> EnableTwoFactorAsync();
        Task<Response<SecuritySettings>> DisableTwoFactorAsync();
        Task<Response<List<SessionInfo>>> GetSessionsAsync();
        Task<Response<bool>> RevokeSessionAsync(string id);
        Task<Response<bool>> RevokeAllSessionsAsync(bool includingCurrent);
        Task<Response<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query);
        Task<Response<bool>> VerifyDeviceAsync(VerifyDeviceRequest request);
    }
}
