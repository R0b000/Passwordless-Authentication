using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.Entities;

namespace PasswordlessApi.Api.Service.Interface.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request);
        Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request);
        Task<Fido2ChallengeResponse> CreateFido2ChallengeAsync(Fido2ChallengeRequest request);
        Task<Fido2VerifyResponse> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<List<UserCredential>> GetUserCredentialsAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<OtpResponse> RequestOtpAsync(OtpRequest request);
        Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request);
    }
}
