using API.Shared.Models.RequestModel.Auth;
using API.Shared.Models.ResponseModel.Auth;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Auth
{
    public interface IOtpService
    {
        Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
    }
}
