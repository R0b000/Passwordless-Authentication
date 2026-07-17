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
            try
            {
                var result = await _httpService.GetAsync<UserProfile>(AccountRoute.Profile);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<UserProfile>.Success(result.Data, "Profile retrieved");
                }
                else
                {
                    return Response<UserProfile>.Fail(result.Messages ?? "Failed to retrieve profile");
                }
            }
            catch (Exception ex)
            {
                return Response<UserProfile>.Fail($"An error occurred while retrieving the profile: {ex.Message}");
            }
        }

        public async Task<IResponse<UserProfile>> UpdateProfileAsync(UserProfile profile)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<UserProfile>(AccountRoute.Profile, profile);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<UserProfile>.Success(result.Data, "Profile updated");
                }
                else
                {
                    return Response<UserProfile>.Fail(result.Messages ?? "Failed to update profile");
                }
            }
            catch (Exception ex)
            {
                return Response<UserProfile>.Fail($"An error occurred while updating the profile: {ex.Message}");
            }
        }

        public async Task<IResponse<AccountSettings>> GetSettingsAsync()
        {
            try
            {
                var result = await _httpService.GetAsync<AccountSettings>(AccountRoute.Settings);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<AccountSettings>.Success(result.Data, "Settings retrieved");
                }
                else
                {
                    return Response<AccountSettings>.Fail(result.Messages ?? "Failed to retrieve settings");
                }
            }
            catch (Exception ex)
            {
                return Response<AccountSettings>.Fail($"An error occurred while retrieving the settings: {ex.Message}");
            }
        }

        public async Task<IResponse<AccountSettings>> UpdateSettingsAsync(AccountSettings settings)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<AccountSettings>(AccountRoute.Settings, settings);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<AccountSettings>.Success(result.Data, "Settings updated");
                }
                else
                {
                    return Response<AccountSettings>.Fail(result.Messages ?? "Failed to update settings");
                }
            }
            catch (Exception ex)
            {
                return Response<AccountSettings>.Fail($"An error occurred while updating the settings: {ex.Message}");
            }
        }

        public async Task<IResponse<PrivacySettings>> GetPrivacyAsync()
        {
            try
            {
                var result = await _httpService.GetAsync<PrivacySettings>(AccountRoute.Privacy);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<PrivacySettings>.Success(result.Data, "Privacy settings retrieved");
                }
                else
                {
                    return Response<PrivacySettings>.Fail(result.Messages ?? "Failed to retrieve privacy settings");
                }
            }
            catch (Exception ex)
            {
                return Response<PrivacySettings>.Fail($"An error occurred while retrieving the privacy settings: {ex.Message}");
            }
        }

        public async Task<IResponse<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<PrivacySettings>(AccountRoute.Privacy, privacy);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<PrivacySettings>.Success(result.Data, "Privacy settings updated");
                }
                else
                {
                    return Response<PrivacySettings>.Fail(result.Messages ?? "Failed to update privacy settings");
                }
            }
            catch (Exception ex)
            {
                return Response<PrivacySettings>.Fail($"An error occurred while updating the privacy settings: {ex.Message}");
            }
        }

        public async Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.Register, request);
                if (result.Succeeded && result.Data?.Token is not null)
                {
                    _tokenStore.SetToken(result.Data.Token);
                    return Response<AuthResponse>.Success(result.Data, "Registration successful");
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages ?? "Registration failed");
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail($"An error occurred during registration: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> RequestPasswordResetAsync(string email)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<ActionResponse>(AccountRoute.PasswordReset, new { email });
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Request sent");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to request password reset");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while requesting the password reset: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<ActionResponse>(
                    AccountRoute.PasswordResetConfirm, new { token, newPassword });
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Password reset");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to reset password");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while resetting the password: {ex.Message}");
            }
        }

        public async Task<IResponse<string>> DownloadDataAsync()
        {
            try
            {
                var result = await _httpService.GetAsync<string>(AccountRoute.DataExport);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<string>.Success(result.Data, "Export prepared");
                }
                else
                {
                    return Response<string>.Fail(result.Messages ?? "Failed to download data");
                }
            }
            catch (Exception ex)
            {
                return Response<string>.Fail($"An error occurred while downloading the data: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> DeleteAccountAsync()
        {
            try
            {
                var result = await _httpService.DeleteAsync<ActionResponse>(AccountRoute.Delete);
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Account deleted");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to delete account");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while deleting the account: {ex.Message}");
            }
        }
    }
}
