using Auth.Model.Models.Auth;
using Shared.Data.Wrapper;

namespace Shared.API.Service.Interface
{
    public interface IOtpService
    {
        Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
    }
}


