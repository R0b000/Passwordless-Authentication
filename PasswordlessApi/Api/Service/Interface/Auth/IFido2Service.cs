using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;

namespace PasswordlessApi.Api.Service.Interface.Auth
{
    public interface IFido2Service
    {
        Task<Fido2ChallengeResponse> CreateChallengeAsync(int userId);
        Task<Fido2VerifyResponse> VerifyAssertionAsync(Fido2VerifyRequest request);
    }
}
