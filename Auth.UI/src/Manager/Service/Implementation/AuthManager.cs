using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Auth;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class AuthManager : IAuthManager
    {
        private readonly GenericHttpRepository<AuthResponse> _authRepository;
        private readonly GenericHttpRepository<Fido2ChallengeResponse> _challengeRepository;
        private readonly GenericHttpRepository<Fido2VerifyResponse> _verifyRepository;
        private readonly GenericHttpRepository<OtpResponse> _otpRepository;
        private readonly ITokenStore _tokenStore;

        public AuthManager(
            GenericHttpRepository<AuthResponse> authRepository,
            GenericHttpRepository<Fido2ChallengeResponse> challengeRepository,
            GenericHttpRepository<Fido2VerifyResponse> verifyRepository,
            GenericHttpRepository<OtpResponse> otpRepository,
            ITokenStore tokenStore)
        {
            _authRepository = authRepository;
            _challengeRepository = challengeRepository;
            _verifyRepository = verifyRepository;
            _otpRepository = otpRepository;
            _tokenStore = tokenStore;
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

        public async Task<Response<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var result = await _authRepository.QuerySingleAsync("api/auth/login", request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<Response<AuthResponse>> GetCurrentUserAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<AuthResponse>.Failure("No authentication token present");
            }

            return await _authRepository.GetSingleAsync("api/auth/me", token);
        }

        public async Task<Response<AuthResponse>> GetUserByEmailAsync(string email)
        {
            return await _authRepository.GetSingleAsync($"api/auth/lookup?email={Uri.EscapeDataString(email)}");
        }

        public async Task<Response<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            var token = _tokenStore.GetToken();
            var result = await _challengeRepository.QuerySingleAsync("api/auth/fido2/options/register", request, token);
            return result;
        }

        public async Task<Response<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            var token = _tokenStore.GetToken();
            var result = await _verifyRepository.QuerySingleAsync("api/auth/fido2/register", request, token);
            return result;
        }

        public async Task<Response<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId, string origin)
        {
            var token = _tokenStore.GetToken();
            var result = await _challengeRepository.QuerySingleAsync("api/auth/fido2/challenge", new Fido2ChallengeRequest { UserId = userId, Origin = origin }, token);
            return result;
        }

        public async Task<Response<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            var token = _tokenStore.GetToken();
            var result = await _verifyRepository.QuerySingleAsync("api/auth/fido2/verify", request, token);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }

        public async Task<Response<OtpResponse>> RequestOtpAsync(OtpRequest request)
        {
            var result = await _otpRepository.QuerySingleAsync("api/auth/otp/request", request);
            return result;
        }

        public async Task<Response<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request)
        {
            var result = await _authRepository.QuerySingleAsync("api/auth/otp/verify", request);
            if (result.Succeeded && result.Data?.Token is not null)
            {
                _tokenStore.SetToken(result.Data.Token);
            }

            return result;
        }
    }
}
