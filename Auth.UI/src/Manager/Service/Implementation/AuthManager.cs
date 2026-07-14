using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Auth;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class AuthManager : IAuthManager
    {
        private readonly IHttpService _httpService;
        private readonly ITokenStore _tokenStore;

        public AuthManager(IHttpService httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
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

        public async Task<Response<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var result = await _httpService.PostAsync<LoginRequest, AuthResponse>(AuthRoute.Login, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<Response<AuthResponse>> GetCurrentUserAsync()
        {
            return await _httpService.GetAsync<AuthResponse>(AuthRoute.Me);
        }

        public async Task<Response<AuthResponse>> GetUserByEmailAsync(string email)
        {
            return await _httpService.GetAsync<AuthResponse>($"{AuthRoute.Lookup}?email={Uri.EscapeDataString(email)}");
        }

        public async Task<Response<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            return await _httpService.PostAsync<Fido2AttestationOptionsRequest, Fido2ChallengeResponse>(AuthRoute.Fido2Options, request);
        }

        public async Task<Response<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            return await _httpService.PostAsync<Fido2RegisterRequest, Fido2VerifyResponse>(AuthRoute.Fido2Register, request);
        }

        public async Task<Response<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId, string origin)
        {
            return await _httpService.PostAsync<Fido2ChallengeRequest, Fido2ChallengeResponse>(
                AuthRoute.Fido2Challenge, new Fido2ChallengeRequest { UserId = userId, Origin = origin });
        }

        public async Task<Response<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            var result = await _httpService.PostAsync<Fido2VerifyRequest, Fido2VerifyResponse>(AuthRoute.Fido2Verify, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<Response<OtpResponse>> RequestOtpAsync(OtpRequest request)
        {
            return await _httpService.PostAsync<OtpRequest, OtpResponse>(AuthRoute.OtpRequest, request);
        }

        public async Task<Response<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request)
        {
            var result = await _httpService.PostAsync<OtpVerifyRequest, AuthResponse>(AuthRoute.OtpVerify, request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }
    }
}
