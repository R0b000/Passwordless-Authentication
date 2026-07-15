using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Security;

namespace Auth.UI.src.Manager.Controller
{
    public class SecurityController
    {
        private readonly ISecurityManager _manager;

        public SecurityController(ISecurityManager manager)
        {
            _manager = manager;
        }

        public Task<IResponse<SecuritySettings>> GetSecurityAsync() => _manager.GetSecurityAsync();
        public Task<IResponse<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings) => _manager.UpdateSecurityAsync(settings);
        public Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request) => _manager.ChangePasswordAsync(request);
        public Task<IResponse<SecuritySettings>> EnableTwoFactorAsync() => _manager.EnableTwoFactorAsync();
        public Task<IResponse<SecuritySettings>> DisableTwoFactorAsync() => _manager.DisableTwoFactorAsync();
        public Task<IResponse<List<SessionInfo>>> GetSessionsAsync() => _manager.GetSessionsAsync();
        public Task<IResponse<bool>> RevokeSessionAsync(string id) => _manager.RevokeSessionAsync(id);
        public Task<IResponse<bool>> RevokeAllSessionsAsync(bool includingCurrent) => _manager.RevokeAllSessionsAsync(includingCurrent);
        public Task<IResponse<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query) => _manager.GetActivityAsync(query);
        public Task<IResponse<bool>> VerifyDeviceAsync(VerifyDeviceRequest request) => _manager.VerifyDeviceAsync(request);
    }
}
