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

        public Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
            => _authManager.RegisterAsync(request);

        public Task<Response<AuthResponse>> LoginAsync(LoginRequest request)
            => _authManager.LoginAsync(request);

        public Task<Response<AuthResponse>> MeAsync()
            => _authManager.GetCurrentUserAsync();

        public Task<Response<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
            => _authManager.RequestAttestationOptionsAsync(request);

        public Task<Response<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
            => _authManager.RegisterCredentialAsync(request);

        public Task<Response<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId)
            => _authManager.CreateFido2ChallengeAsync(userId);

        public Task<Response<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
            => _authManager.VerifyFido2AssertionAsync(request);
    }
}
