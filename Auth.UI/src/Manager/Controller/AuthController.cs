using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Controller
{
    public class AuthController
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        public Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
            => _authManager.RegisterAsync(request);

        public Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request)
            => _authManager.LoginAsync(request);

        public Task<IResponse<AuthResponse>> MeAsync()
            => _authManager.GetCurrentUserAsync();

        public Task<IResponse<AuthResponse>> GetUserByEmailAsync(string email)
            => _authManager.GetUserByEmailAsync(email);

        public Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
            => _authManager.RequestAttestationOptionsAsync(request);

        public Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
            => _authManager.RegisterCredentialAsync(request);

        public Task<IResponse<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId, string origin)
            => _authManager.CreateFido2ChallengeAsync(userId, origin);

        public Task<IResponse<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
            => _authManager.VerifyFido2AssertionAsync(request);

        public Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request)
            => _authManager.RequestOtpAsync(request);

        public Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request)
            => _authManager.VerifyOtpAsync(request);
    }
}
