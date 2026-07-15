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

        public Task<IResponse<UserProfile>> GetProfileAsync() => _manager.GetProfileAsync();
        public Task<IResponse<UserProfile>> UpdateProfileAsync(UserProfile profile) => _manager.UpdateProfileAsync(profile);
        public Task<IResponse<AccountSettings>> GetSettingsAsync() => _manager.GetSettingsAsync();
        public Task<IResponse<AccountSettings>> UpdateSettingsAsync(AccountSettings settings) => _manager.UpdateSettingsAsync(settings);
        public Task<IResponse<PrivacySettings>> GetPrivacyAsync() => _manager.GetPrivacyAsync();
        public Task<IResponse<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy) => _manager.UpdatePrivacyAsync(privacy);
        public Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request) => _manager.RegisterAsync(request);
        public Task<IResponse<bool>> RequestPasswordResetAsync(string email) => _manager.RequestPasswordResetAsync(email);
        public Task<IResponse<bool>> ResetPasswordAsync(string token, string newPassword) => _manager.ResetPasswordAsync(token, newPassword);
        public Task<IResponse<string>> DownloadDataAsync() => _manager.DownloadDataAsync();
        public Task<IResponse<bool>> DeleteAccountAsync() => _manager.DeleteAccountAsync();
    }
}
