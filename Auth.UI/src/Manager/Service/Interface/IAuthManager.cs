using Auth.UI.src.Common;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Service.Interface
{
    public interface IAuthManager
    {
        Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<Response<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Response<AuthResponse>> GetCurrentUserAsync();
        Task<Response<AuthResponse>> GetUserByEmailAsync(string email);
        Task<Response<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request);
        Task<Response<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request);
        Task<Response<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId);
        Task<Response<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<Response<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<Response<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
    }
}
