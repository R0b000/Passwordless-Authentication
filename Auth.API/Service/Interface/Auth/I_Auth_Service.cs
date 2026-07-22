using Auth.Model.Models.Common;
using Auth.Model.Models.Entities;
using Auth.Model.Models.Account;
using Auth.Model.Models.Auth;
using Auth.Model.Models.Security;
using Auth.Model.Models.Rbac;
using Shared.Data.Wrapper;

namespace Auth.API.Service.Interface.Auth
{
    public interface IAuthService
    {
        Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress = null, string? userAgent = null);
        Task<IResponse<User?>> GetUserByIdAsync(int userId);
        Task<IResponse<User?>> GetUserByEmailAsync(string email);
        Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request);
        Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request);
        Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request);
        Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request);
        Task<IResponse<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(Fido2ChallengeRequest request);
        Task<IResponse<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request);
        Task<IResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<IResponse<ActiveSessionsResponse>> GetActiveSessionsAsync(int userId, int currentRefreshTokenId = 0);
        Task<IResponse> RevokeAllSessionsAsync(int userId);
        Task<IResponse> RevokeSessionAsync(int sessionId, int userId);
        Task<IResponse> RequestPasswordResetAsync(string email);

        Task<IResponse<UserProfileResponse?>> GetProfileAsync(int userId);
        Task<IResponse<UserProfileResponse?>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<IResponse<AccountSettingsResponse>> GetAccountSettingsAsync(int userId);
        Task<IResponse<AccountSettingsResponse>> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request);
        Task<IResponse<PrivacySettingsResponse>> GetPrivacySettingsAsync(int userId);
        Task<IResponse<PrivacySettingsResponse>> UpdatePrivacySettingsAsync(int userId, UpdatePrivacyRequest request);
        Task<IResponse<string>> GetUserDataExportAsync(int userId);
        Task<IResponse<SecuritySettingsResponse>> GetSecuritySettingsAsync(int userId);
        Task<IResponse<SecuritySettingsResponse>> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request);
        Task<IResponse<SecuritySettingsResponse>> EnableTwoFactorAsync(int userId);
        Task<IResponse<SecuritySettingsResponse>> DisableTwoFactorAsync(int userId);
        Task<IResponse<ActivityLogResponse>> GetActivityLogsAsync(int userId, ActivityQueryRequest query);
        Task<IResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<IResponse> ResetPasswordAsync(string token, string newPassword);
        Task<IResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request);
        Task<IResponse> DeleteAccountAsync(int userId);
    }
}

