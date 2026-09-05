using Auth.Model.Models.Auth;
using Shared.Data.Wrapper;

namespace Auth.API.Service.Interface.Auth
{
    public interface IFido2Service
    {
        Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(int userId, string username, string origin, string? appName = null);
        Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request, string origin, string? appName = null);
        Task<IResponse<Fido2ChallengeResponse>> CreateChallengeAsync(int userId, string origin, string? appName = null);
        Task<IResponse<Fido2VerifyResponse>> VerifyAssertionAsync(Fido2VerifyRequest request, string? appName = null);
    }
}

