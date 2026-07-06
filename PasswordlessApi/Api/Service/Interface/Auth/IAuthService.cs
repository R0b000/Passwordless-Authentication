using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.Entities;

namespace PasswordlessApi.Api.Service.Interface.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<Fido2ChallengeResponse> CreateFido2ChallengeAsync(Fido2ChallengeRequest request);
        Task<Fido2VerifyResponse> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<List<UserCredential>> GetUserCredentialsAsync(int userId);
    }
}
