using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;

namespace PasswordlessApi.Api.Service.Interface.Auth
{
    public interface IOtpService
    {
        Task<OtpResponse> RequestOtpAsync(OtpRequest request);
        Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request);
    }
}