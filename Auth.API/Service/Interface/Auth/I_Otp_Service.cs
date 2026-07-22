using Auth.Model.Models.Auth;
using Auth.Model.Wrapper;

namespace Auth.API.Service.Interface.Auth
{
    public interface IOtpService
    {
        Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
    }
}

