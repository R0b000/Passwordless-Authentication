using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Account;
using Auth.UI.src.Model.Auth;
using Auth.UI.src.Model.Security;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class AccountManager : IAccountManager
    {
        private readonly IHttpService _httpService;
        private readonly ITokenStore _tokenStore;

        public AccountManager(IHttpService httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
        }

        public async Task<Response<UserProfile>> GetProfileAsync()
        {
            return await _httpService.GetAsync<UserProfile>(AccountRoute.Profile);
        }

        public async Task<Response<UserProfile>> UpdateProfileAsync(UserProfile profile)
        {
            return await _httpService.PostAsync<UserProfile, UserProfile>(AccountRoute.Profile, profile);
        }

        public async Task<Response<AccountSettings>> GetSettingsAsync()
        {
            return await _httpService.GetAsync<AccountSettings>(AccountRoute.Settings);
        }

        public async Task<Response<AccountSettings>> UpdateSettingsAsync(AccountSettings settings)
        {
            return await _httpService.PostAsync<AccountSettings, AccountSettings>(AccountRoute.Settings, settings);
        }

        public async Task<Response<PrivacySettings>> GetPrivacyAsync()
        {
            return await _httpService.GetAsync<PrivacySettings>(AccountRoute.Privacy);
        }

        public async Task<Response<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy)
        {
            return await _httpService.PostAsync<PrivacySettings, PrivacySettings>(AccountRoute.Privacy, privacy);
        }

        public async Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var result = await _httpService.PostAsync<RegisterRequest, AuthResponse>(AuthRoute.Register, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<Response<bool>> RequestPasswordResetAsync(string email)
        {
            var result = await _httpService.PostAsync<object, ActionResponse>(AccountRoute.PasswordReset, new { email });
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Request sent");
        }

        public async Task<Response<bool>> ResetPasswordAsync(string token, string newPassword)
        {
            var result = await _httpService.PostAsync<object, ActionResponse>(
                AccountRoute.PasswordResetConfirm, new { token, newPassword });
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Password reset");
        }

        public async Task<Response<string>> DownloadDataAsync()
        {
            var result = await _httpService.GetAsync<string>(AccountRoute.DataExport);
            if (!result.Succeeded || result.Data is null)
            {
                return Response<string>.Failure(result.Message ?? "Failed to download data");
            }

            return Response<string>.Success(result.Data, "Export prepared");
        }

        public async Task<Response<bool>> DeleteAccountAsync()
        {
            var result = await _httpService.DeleteAsync<ActionResponse>(AccountRoute.Delete);
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Account deleted");
        }
    }
}
