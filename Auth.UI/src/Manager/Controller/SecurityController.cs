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

        public Task<Response<SecuritySettings>> GetSecurityAsync() => _manager.GetSecurityAsync();
        public Task<Response<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings) => _manager.UpdateSecurityAsync(settings);
        public Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest request) => _manager.ChangePasswordAsync(request);
        public Task<Response<SecuritySettings>> EnableTwoFactorAsync() => _manager.EnableTwoFactorAsync();
        public Task<Response<SecuritySettings>> DisableTwoFactorAsync() => _manager.DisableTwoFactorAsync();
        public Task<Response<List<SessionInfo>>> GetSessionsAsync() => _manager.GetSessionsAsync();
        public Task<Response<bool>> RevokeSessionAsync(string id) => _manager.RevokeSessionAsync(id);
        public Task<Response<bool>> RevokeAllSessionsAsync(bool includingCurrent) => _manager.RevokeAllSessionsAsync(includingCurrent);
        public Task<Response<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query) => _manager.GetActivityAsync(query);
        public Task<Response<bool>> VerifyDeviceAsync(VerifyDeviceRequest request) => _manager.VerifyDeviceAsync(request);
    }
}
