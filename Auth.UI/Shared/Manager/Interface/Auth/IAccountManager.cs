using Shared.Wrapper;
using Auth.UI.Shared.Model.Account;
using Auth.UI.Shared.Model.Auth;

namespace UI.Shared.Manager.Interface.Auth
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
