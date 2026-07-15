using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Auth;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class AuthManager : IAuthManager
    {
        private readonly IHttpServices _httpService;
        private readonly ITokenStore _tokenStore;

        public AuthManager(IHttpServices httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
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

        public async Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.Login, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<IResponse<AuthResponse>> GetCurrentUserAsync()
        {
            return await _httpService.GetAsync<AuthResponse>(AuthRoute.Me);
        }

        public async Task<IResponse<AuthResponse>> GetUserByEmailAsync(string email)
        {
            return await _httpService.GetAsync<AuthResponse>($"{AuthRoute.Lookup}?email={Uri.EscapeDataString(email)}");
        }

        public async Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            return await _httpService.PostAsJsonAsync<Fido2ChallengeResponse>(AuthRoute.Fido2Options, request);
        }

        public async Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            return await _httpService.PostAsJsonAsync<Fido2VerifyResponse>(AuthRoute.Fido2Register, request);
        }

        public async Task<IResponse<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId, string origin)
        {
            return await _httpService.PostAsJsonAsync<Fido2ChallengeResponse>(
                AuthRoute.Fido2Challenge, new Fido2ChallengeRequest { UserId = userId, Origin = origin });
        }

        public async Task<IResponse<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            var result = await _httpService.PostAsJsonAsync<Fido2VerifyResponse>(AuthRoute.Fido2Verify, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request)
        {
            return await _httpService.PostAsJsonAsync<OtpResponse>(AuthRoute.OtpRequest, request);
        }

        public async Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request)
        {
            var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.OtpVerify, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }
    }
}
