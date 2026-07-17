using API.Shared.Models.Common;
using API.Shared.Models.Entities;
using API.Shared.Models.RequestModel.Account;
using API.Shared.Models.RequestModel.Auth;
using API.Shared.Models.RequestModel.Security;
using API.Shared.Models.ResponseModel.Account;
using API.Shared.Models.ResponseModel.Auth;
using API.Shared.Models.ResponseModel.Security;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<Response<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress = null, string? userAgent = null);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<OtpResponse> RequestOtpAsync(OtpRequest request);
        Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request);
        Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request);
        Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request);
        Task<Fido2ChallengeResponse> CreateFido2ChallengeAsync(Fido2ChallengeRequest request);
        Task<Fido2VerifyResponse> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ActiveSessionsResponse> GetActiveSessionsAsync(int userId, int currentRefreshTokenId = 0);
        Task RevokeAllSessionsAsync(int userId);
        Task RevokeSessionAsync(int sessionId, int userId);

        Task<UserProfileResponse?> GetProfileAsync(int userId);
        Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<AccountSettingsResponse> GetAccountSettingsAsync(int userId);
        Task<AccountSettingsResponse> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request);
        Task<PrivacySettingsResponse> GetPrivacySettingsAsync(int userId);
        Task<PrivacySettingsResponse> UpdatePrivacySettingsAsync(int userId, UpdatePrivacyRequest request);
        Task RequestPasswordResetAsync(string email);
        Task<IResponse> ResetPasswordAsync(string token, string newPassword);
        Task<string> GetUserDataExportAsync(int userId);
        Task<IResponse> DeleteAccountAsync(int userId);

        Task<SecuritySettingsResponse> GetSecuritySettingsAsync(int userId);
        Task<SecuritySettingsResponse> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request);
        Task<IResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<SecuritySettingsResponse> EnableTwoFactorAsync(int userId);
        Task<SecuritySettingsResponse> DisableTwoFactorAsync(int userId);
        Task<ActivityLogResponse> GetActivityLogsAsync(int userId, ActivityQueryRequest query);
        Task<IResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request);
    }
}