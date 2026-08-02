using Shared.Data.Wrapper;
using Auth.Model.Models.Account;
using Auth.Model.Models.Auth;

namespace Auth.UI.Manager.Interface.Auth
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
        Task<IResponse<bool>> RequestPasswordResetAsync(ForgotPasswordRequest request);
        Task<IResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<IResponse<string>> DownloadDataAsync();
        Task<IResponse<bool>> DeleteAccountAsync();
    }
}