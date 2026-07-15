using API.Shared.Models.RequestModel.Auth;
using API.Shared.Models.ResponseModel.Auth;

namespace API.Shared.Service.Interface.Auth
{
    public interface IOtpService
    {
        Task<OtpResponse> RequestOtpAsync(OtpRequest request);
        Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request);
    }
}