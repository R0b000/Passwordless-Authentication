using Auth.UI.src.Common;
using Auth.UI.src.Model.Account;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Service.Interface
{
    public interface IAccountManager
    {
        Task<IResponse<UserProfile>> GetProfileAsync();
        Task<IResponse<UserProfile>> UpdateProfileAsync(UserProfile profile);
        Task<IResponse<AccountSettings>> GetSettingsAsync();
        Task<IResponse<AccountSettings>> UpdateSettingsAsync(AccountSettings settings);
        Task<IResponse<PrivacySettings>> GetPrivacyAsync();
        Task<IResponse<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy);
        Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<IResponse<bool>> RequestPasswordResetAsync(string email);
        Task<IResponse<bool>> ResetPasswordAsync(string token, string newPassword);
        Task<IResponse<string>> DownloadDataAsync();
        Task<IResponse<bool>> DeleteAccountAsync();
    }
}
