using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.RequestModel.Security;
using PasswordlessApi.Api.Models.RequestModel.Account;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Security;
using PasswordlessApi.Api.Models.ResponseModel.Account;
using PasswordlessApi.Api.Models.Common;
using PasswordlessApi.Api.Models.Entities;

namespace PasswordlessApi.Api.Service.Interface.Auth
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
        Task<MessageResponse> ResetPasswordAsync(string token, string newPassword);
        Task<string> GetUserDataExportAsync(int userId);
        Task<MessageResponse> DeleteAccountAsync(int userId);

        Task<SecuritySettingsResponse> GetSecuritySettingsAsync(int userId);
        Task<SecuritySettingsResponse> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request);
        Task<MessageResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<SecuritySettingsResponse> EnableTwoFactorAsync(int userId);
        Task<SecuritySettingsResponse> DisableTwoFactorAsync(int userId);
        Task<ActivityLogResponse> GetActivityLogsAsync(int userId, ActivityQueryRequest query);
        Task<MessageResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request);
    }
}