using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.RequestModel.Security;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Security;
using PasswordlessApi.Api.Models.Entities;

namespace PasswordlessApi.Api.Service.Interface.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress = null, string? userAgent = null);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<OtpResponse> RequestOtpAsync(OtpRequest request);
        Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request);
        Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request);
        Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request);
        Task<Fido2ChallengeResponse> CreateFido2ChallengeAsync(Fido2ChallengeRequest request);
        Task<Fido2VerifyResponse> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ActiveSessionsResponse> GetActiveSessionsAsync(int userId, int currentRefreshTokenId = 0);
        Task RevokeAllSessionsAsync(int userId);
        Task RevokeSessionAsync(int sessionId, int userId);
    }
}