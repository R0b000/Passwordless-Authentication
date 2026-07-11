using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Account;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Controller
{
    public class AccountController
    {
        private readonly IAccountManager _manager;

        public AccountController(IAccountManager manager)
        {
            _manager = manager;
        }

        public Task<Response<UserProfile>> GetProfileAsync() => _manager.GetProfileAsync();
        public Task<Response<UserProfile>> UpdateProfileAsync(UserProfile profile) => _manager.UpdateProfileAsync(profile);
        public Task<Response<AccountSettings>> GetSettingsAsync() => _manager.GetSettingsAsync();
        public Task<Response<AccountSettings>> UpdateSettingsAsync(AccountSettings settings) => _manager.UpdateSettingsAsync(settings);
        public Task<Response<PrivacySettings>> GetPrivacyAsync() => _manager.GetPrivacyAsync();
        public Task<Response<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy) => _manager.UpdatePrivacyAsync(privacy);
        public Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request) => _manager.RegisterAsync(request);
        public Task<Response<bool>> RequestPasswordResetAsync(string email) => _manager.RequestPasswordResetAsync(email);
        public Task<Response<bool>> ResetPasswordAsync(string token, string newPassword) => _manager.ResetPasswordAsync(token, newPassword);
        public Task<Response<string>> DownloadDataAsync() => _manager.DownloadDataAsync();
        public Task<Response<bool>> DeleteAccountAsync() => _manager.DeleteAccountAsync();
    }
}
