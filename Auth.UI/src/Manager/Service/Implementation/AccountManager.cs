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
        private readonly GenericHttpRepository<UserProfile> _profileRepository;
        private readonly GenericHttpRepository<AccountSettings> _settingsRepository;
        private readonly GenericHttpRepository<PrivacySettings> _privacyRepository;
        private readonly GenericHttpRepository<AuthResponse> _authRepository;
        private readonly GenericHttpRepository<ActionResponse> _actionRepository;
        private readonly GenericHttpRepository<string> _stringRepository;
        private readonly ITokenStore _tokenStore;

        public AccountManager(
            GenericHttpRepository<UserProfile> profileRepository,
            GenericHttpRepository<AccountSettings> settingsRepository,
            GenericHttpRepository<PrivacySettings> privacyRepository,
            GenericHttpRepository<AuthResponse> authRepository,
            GenericHttpRepository<ActionResponse> actionRepository,
            GenericHttpRepository<string> stringRepository,
            ITokenStore tokenStore)
        {
            _profileRepository = profileRepository;
            _settingsRepository = settingsRepository;
            _privacyRepository = privacyRepository;
            _authRepository = authRepository;
            _actionRepository = actionRepository;
            _stringRepository = stringRepository;
            _tokenStore = tokenStore;
        }

        public async Task<Response<UserProfile>> GetProfileAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<UserProfile>.Failure("No authentication token present");
            }

            return await _profileRepository.GetSingleAsync("api/account/profile", token);
        }

        public async Task<Response<UserProfile>> UpdateProfileAsync(UserProfile profile)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<UserProfile>.Failure("No authentication token present");
            }

            return await _profileRepository.QuerySingleAsync("api/account/profile", profile, token);
        }

        public async Task<Response<AccountSettings>> GetSettingsAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<AccountSettings>.Failure("No authentication token present");
            }

            return await _settingsRepository.GetSingleAsync("api/account/settings", token);
        }

        public async Task<Response<AccountSettings>> UpdateSettingsAsync(AccountSettings settings)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<AccountSettings>.Failure("No authentication token present");
            }

            return await _settingsRepository.QuerySingleAsync("api/account/settings", settings, token);
        }

        public async Task<Response<PrivacySettings>> GetPrivacyAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<PrivacySettings>.Failure("No authentication token present");
            }

            return await _privacyRepository.GetSingleAsync("api/account/privacy", token);
        }

        public async Task<Response<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<PrivacySettings>.Failure("No authentication token present");
            }

            return await _privacyRepository.QuerySingleAsync("api/account/privacy", privacy, token);
        }

        public async Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var result = await _authRepository.QuerySingleAsync("api/auth/register", request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<Response<bool>> RequestPasswordResetAsync(string email)
        {
            var token = _tokenStore.GetToken();
            var result = await _actionRepository.QuerySingleAsync("api/account/password-reset", new { email }, token);
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Request sent");
        }

        public async Task<Response<bool>> ResetPasswordAsync(string token, string newPassword)
        {
            var result = await _actionRepository.QuerySingleAsync("api/account/password-reset/confirm", new { token, newPassword });
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Password reset");
        }

        public async Task<Response<string>> DownloadDataAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<string>.Failure("No authentication token present");
            }

            var result = await _stringRepository.GetSingleAsync("api/account/data-export", token);
            if (result == null)
            {
                return Response<string>.Failure("Failed to download data");
            }

            return Response<string>.Success(result.Data, "Export prepared");
        }

        public async Task<Response<bool>> DeleteAccountAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<bool>.Failure("No authentication token present");
            }

            var result = await _actionRepository.QuerySingleAsync("api/account", null, token);
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Account deleted");
        }
    }
}
