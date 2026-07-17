using Shared.Wrapper;
using Auth.UI.Shared.Model.Account;
using Auth.UI.Shared.Model.Auth;
using Auth.UI.Shared.Model.Security;
using Auth.UI.Shared.Utility;
using UI.Shared.Manager.Interface.Auth;
using UI.Shared.Manager.Interface.Http;
using UI.Shared.Manager.Routes;

namespace UI.Shared.Manager.Implementation.Auth
{
    public class AccountManager : IAccountManager
    {
        private readonly IHttpServices _httpService;
        private readonly ITokenStore _tokenStore;

        public AccountManager(IHttpServices httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
        }

        public async Task<IResponse<UserProfile>> GetProfileAsync()
        {
            return await _httpService.GetAsync<UserProfile>(AccountRoute.Profile);
        }

        public async Task<IResponse<UserProfile>> UpdateProfileAsync(UserProfile profile)
        {
            return await _httpService.PostAsJsonAsync<UserProfile>(AccountRoute.Profile, profile);
        }

        public async Task<IResponse<AccountSettings>> GetSettingsAsync()
        {
            return await _httpService.GetAsync<AccountSettings>(AccountRoute.Settings);
        }

        public async Task<IResponse<AccountSettings>> UpdateSettingsAsync(AccountSettings settings)
        {
            return await _httpService.PostAsJsonAsync<AccountSettings>(AccountRoute.Settings, settings);
        }

        public async Task<IResponse<PrivacySettings>> GetPrivacyAsync()
        {
            return await _httpService.GetAsync<PrivacySettings>(AccountRoute.Privacy);
        }

        public async Task<IResponse<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy)
        {
            return await _httpService.PostAsJsonAsync<PrivacySettings>(AccountRoute.Privacy, privacy);
        }

        public async Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.Register, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<IResponse<bool>> RequestPasswordResetAsync(string email)
        {
            var result = await _httpService.PostAsJsonAsync<ActionResponse>(AccountRoute.PasswordReset, new { email });
            return Response<bool>.Success(result.Succeeded, result.Messages ?? "Request sent");
        }

        public async Task<IResponse<bool>> ResetPasswordAsync(string token, string newPassword)
        {
            var result = await _httpService.PostAsJsonAsync<ActionResponse>(
                AccountRoute.PasswordResetConfirm, new { token, newPassword });
            return Response<bool>.Success(result.Succeeded, result.Messages ?? "Password reset");
        }

        public async Task<IResponse<string>> DownloadDataAsync()
        {
            var result = await _httpService.GetAsync<string>(AccountRoute.DataExport);
            if (!result.Succeeded || result.Data is null)
            {
                return Response<string>.Fail(result.Messages ?? "Failed to download data");
            }

            return Response<string>.Success(result.Data, "Export prepared");
        }

        public async Task<IResponse<bool>> DeleteAccountAsync()
        {
            var result = await _httpService.DeleteAsync<ActionResponse>(AccountRoute.Delete);
            return Response<bool>.Success(result.Succeeded, result.Messages ?? "Account deleted");
        }
    }
}
