using Auth.UI.src.Common;
using Auth.UI.src.Model.Account;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Service.Interface
{
    public interface IAccountManager
    {
        Task<Response<UserProfile>> GetProfileAsync();
        Task<Response<UserProfile>> UpdateProfileAsync(UserProfile profile);
        Task<Response<AccountSettings>> GetSettingsAsync();
        Task<Response<AccountSettings>> UpdateSettingsAsync(AccountSettings settings);
        Task<Response<PrivacySettings>> GetPrivacyAsync();
        Task<Response<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy);
        Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<Response<bool>> RequestPasswordResetAsync(string email);
        Task<Response<bool>> ResetPasswordAsync(string token, string newPassword);
        Task<Response<string>> DownloadDataAsync();
        Task<Response<bool>> DeleteAccountAsync();
    }
}
