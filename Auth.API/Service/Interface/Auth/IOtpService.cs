using Shared.Core.Models.Auth;
using Shared.Core.Wrapper;

namespace Auth.API.Service.Interface.Auth
{
    public interface IOtpService
    {
        Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
    }
}
