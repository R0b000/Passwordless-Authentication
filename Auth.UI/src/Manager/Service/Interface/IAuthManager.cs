using Auth.UI.src.Common;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Service.Interface
{
    public interface IAuthManager
    {
        Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<IResponse<AuthResponse>> GetCurrentUserAsync();
        Task<IResponse<AuthResponse>> GetUserByEmailAsync(string email);
        Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request);
        Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request);
        Task<IResponse<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId, string origin);
        Task<IResponse<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
    }
}
