using API.Shared.Models.RequestModel.Auth;
using API.Shared.Models.ResponseModel.Auth;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Auth
{
    public interface IFido2Service
    {
        Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(int userId, string username, string origin);
        Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request, string origin);
        Task<IResponse<Fido2ChallengeResponse>> CreateChallengeAsync(int userId, string origin);
        Task<IResponse<Fido2VerifyResponse>> VerifyAssertionAsync(Fido2VerifyRequest request, string origin);
    }
}