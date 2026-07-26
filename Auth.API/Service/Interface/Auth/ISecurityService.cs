using Auth.Model.Models.Security;
using Auth.Model.Models.Account;
using Shared.Data.Wrapper;
using Auth.API.Config;
using Auth.API.Utility.PasswordHash;
using Auth.API.Service.Interface.Security;

namespace Auth.API.Service.Interface.Auth
{
    public interface ISecurityService
    {
        Task<IResponse<SecuritySettingsResponse>> GetSecuritySettingsAsync(int userId);
        Task<IResponse<SecuritySettingsResponse>> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request);
        Task<IResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<IResponse<SecuritySettingsResponse>> EnableTwoFactorAsync(int userId);
        Task<IResponse<SecuritySettingsResponse>> DisableTwoFactorAsync(int userId);
        Task<IResponse<ActivityLogResponse>> GetActivityLogsAsync(int userId, ActivityQueryRequest query);
        Task<IResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request);
    }
}
