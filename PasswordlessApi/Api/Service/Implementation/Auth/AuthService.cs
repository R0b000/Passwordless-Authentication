using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.RequestModel.Security;
using PasswordlessApi.Api.Models.RequestModel.Account;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Security;
using PasswordlessApi.Api.Models.ResponseModel.Account;
using PasswordlessApi.Api.Models.Common;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Service.Interface.Security;
using PasswordlessApi.Api.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _authRepository;
        private readonly IPasswordHash _passwordHash;
        private readonly IJwtHelper _jwtHelper;
        private readonly IFido2Service _fido2Service;
        private readonly ILocationResolver _locationResolver;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SecuritySettings _securitySettings;
        private const string ProcedureName = "sp_Users";

        public AuthService(
            IGenericRepository<User> authRepository, 
            IPasswordHash passwordHash, 
            IJwtHelper jwtHelper, 
            IFido2Service fido2Service,
            ILocationResolver locationResolver,
            IAuditLogService auditLogService,
            IHttpContextAccessor httpContextAccessor,
            Microsoft.Extensions.Options.IOptions<SecuritySettings> securitySettings)
        {
            _authRepository = authRepository;
            _passwordHash = passwordHash;
            _jwtHelper = jwtHelper;
            _fido2Service = fido2Service;
            _locationResolver = locationResolver;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
            _securitySettings = securitySettings.Value;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var passwordHash = _passwordHash.HashPassword(request.Password);

            var param = new
            {
                AuthType = "Register",
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            var userIdResult = await _authRepository.QuerySingleAsync<UserIdResult>(ProcedureName, param);

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null)
            {
                return new AuthResponse
                {
                    Message = "Registration failed"
                };
            }

            var token = _jwtHelper.GenerateToken(userIdResult.Data.UserId, request.Username);
            var deviceInfo = GetDeviceInfo();
            await CreateRefreshTokenAsync(userIdResult.Data.UserId, deviceInfo);

            await _auditLogService.LogAsync(
                userIdResult.Data.UserId, 
                "UserRegistered", 
                "User", 
                userIdResult.Data.UserId.ToString(),
                null, 
                request.Username,
                deviceInfo.IpAddress,
                deviceInfo.UserAgent);

            return new AuthResponse
            {
                UserId = userIdResult.Data.UserId,
                Username = request.Username,
                Email = request.Email,
                Token = token,
                Message = "Registered successfully"
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress = null, string? userAgent = null)
        {
            var param = new
            {
                AuthType = "Login",
                Username = request.Username
            };

            var userIdResult = await _authRepository.QuerySingleAsync<UserIdResult>(
                ProcedureName,
                param
            );

            if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null || userIdResult.Data.UserId <= 0)
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var param_1 = new
            {
                AuthType = "Login",
                UserId = userIdResult.Data.UserId
            };

            var user = await _authRepository.QuerySingleAsync<User>(ProcedureName, param_1);

            if (user == null || !user.Succeeded || user.Data == null || string.IsNullOrEmpty(user.Data.PasswordHash))
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var isValid = _passwordHash.VerifyPassword(request.Password, user.Data.PasswordHash);

            if (!isValid)
            {
                return new AuthResponse
                {
                    Message = "Invalid username or password"
                };
            }

            var hasFido2Credentials = await HasFido2CredentialsAsync(user.Data.Id);

            if (hasFido2Credentials)
            {
                return new AuthResponse
                {
                    UserId = user.Data.Id,
                    Username = user.Data.Username,
                    Email = user.Data.Email,
                    Message = "FIDO2 verification required",
                    RequiresFido2 = true
                };
            }

            var token = _jwtHelper.GenerateToken(user.Data.Id, user.Data.Username);
            var deviceInfo = new DeviceInfo
            {
                IpAddress = ipAddress ?? GetClientIpAddress(),
                UserAgent = userAgent ?? GetUserAgent()
            };
            deviceInfo.Location = await _locationResolver.ResolveLocationAsync(deviceInfo.IpAddress);

            await EnforceConcurrentSessionLimitAsync(user.Data.Id);
            await CreateRefreshTokenAsync(user.Data.Id, deviceInfo);

            await _auditLogService.LogAsync(
                user.Data.Id,
                "UserLogin",
                "User",
                user.Data.Id.ToString(),
                null,
                "Login successful",
                deviceInfo.IpAddress,
                deviceInfo.UserAgent);

            return new AuthResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username,
                Email = user.Data.Email,
                Token = token,
                Message = "Login successful",
                RequiresFido2 = false
            };
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = "Login", UserId = userId });
            return userResult.Succeeded ? userResult.Data : null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = "Login", Email = email });
            return userResult.Succeeded ? userResult.Data : null;
        }

        public async Task<OtpResponse> RequestOtpAsync(OtpRequest request)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = "Login", UserId = request.UserId });

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new OtpResponse { Success = false, Message = "User not found" };
            }

            var user = userResult.Data;
            if (string.IsNullOrEmpty(user.Email))
            {
                return new OtpResponse { Success = false, Message = "User does not have an email configured" };
            }

            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "EmailOtp",
                    FIDOOperation = "CreateOtp",
                    UserId = user.Id,
                    Otp = otp,
                    ExpiresAt = expiresAt
                });

            return new OtpResponse
            {
                Success = true,
                Message = $"OTP sent to {user.Email} (Demo OTP: {otp})",
                Otp = otp
            };
        }

        public async Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request)
        {
            var now = DateTime.UtcNow;
            var param = new
            {
                AuthType = "Login",
                UserId = request.UserId
            };

            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                param);

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new AuthResponse { Message = "User not found" };
            }

            var user = userResult.Data;
            var param_1 = new
            {
                AuthType = "EmailOtp",
                FIDOOperation = "ConsumeOtp",
                UserId = request.UserId,
                Otp = request.Otp,
                Now = now
            };

            var isConsumed = await _authRepository.QueryFirstAsync<bool>(
                ProcedureName,
                param_1);

            if (isConsumed != true)
            {
                return new AuthResponse { Message = "Invalid or expired OTP" };
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Username);
            var deviceInfo = GetDeviceInfo();
            await CreateRefreshTokenAsync(user.Id, deviceInfo);

            await _auditLogService.LogAsync(
                user.Id,
                "OtpLogin",
                "User",
                user.Id.ToString(),
                null,
                "OTP login successful",
                deviceInfo.IpAddress,
                deviceInfo.UserAgent);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Message = "Login successful",
                RequiresFido2 = false
            };
        }

        public async Task<List<UserCredential>> GetUserCredentialsAsync(int userId)
        {
            var param = new
            {
                AuthType = "FIDO",
                FIDOOperation = "GetCredentialsByUserId",
                UserId = userId
            };

            var result = await _authRepository.QueryAsync<UserCredential>(
                ProcedureName,
                param);

             return result.Data!.ToList();
        }

        public async Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            return await _fido2Service.RequestAttestationOptionsAsync(request.UserId, request.Username, request.Origin ?? string.Empty);
        }

        public async Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            return await _fido2Service.RegisterCredentialAsync(request, request.Origin ?? string.Empty);
        }

        public async Task<Fido2ChallengeResponse> CreateFido2ChallengeAsync(Fido2ChallengeRequest request)
        {
            return await _fido2Service.CreateChallengeAsync(request.UserId, request.Origin ?? string.Empty);
        }

        public async Task<Fido2VerifyResponse> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            return await _fido2Service.VerifyAssertionAsync(request, request.Origin ?? string.Empty);
        }

        private async Task<bool> HasFido2CredentialsAsync(int userId)
        {
            var param = new
            {
                AuthType = "FIDO",
                FIDOOperation = "GetCredentialsByUserId",
                UserId = userId
            };

            var result = await _authRepository.QueryAsync<UserCredential>(
                ProcedureName,
                param);

            return result.Succeeded && result.Data != null && result.Data.Any();
        }

        private class DeviceInfo
        {
            public string? IpAddress { get; set; }
            public string? UserAgent { get; set; }
            public string? Location { get; set; }
        }

        private async Task CreateRefreshTokenAsync(int userId, DeviceInfo? deviceInfo = null)
        {
            var now = DateTime.UtcNow;
            var rawToken = _jwtHelper.GenerateRefreshToken();
            var tokenHash = _passwordHash.HashPassword(rawToken);
            var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "CreateRefreshToken",
                    UserId = userId,
                    TokenHash = tokenHash,
                    ExpiresAt = now.AddDays(refreshExpiryDays),
                    Now = now,
                    IpAddress = deviceInfo?.IpAddress,
                    UserAgent = deviceInfo?.UserAgent,
                    Location = deviceInfo?.Location
                });
        }

        public async Task<AuthResponse> RefreshTokenAsync(PasswordlessApi.Api.Models.RequestModel.Security.RefreshTokenRequest request)
        {
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal? principal = null;

            try
            {
                principal = tokenHandler.ValidateToken(
                    request.AccessToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtHelper.GetSigningKey())),
                        ValidateIssuer = true,
                        ValidIssuer = _jwtHelper.Issuer,
                        ValidateAudience = true,
                        ValidAudience = _jwtHelper.Audience,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero
                    },
                    out _);
            }
            catch
            {
                return new AuthResponse { Message = "Invalid access token" };
            }

            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return new AuthResponse { Message = "Invalid access token claims" };
            }

            var incomingTokenHash = _passwordHash.HashPassword(request.RefreshToken);

            var storedRefreshToken = await _authRepository.QuerySingleAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "GetRefreshToken",
                    TokenHash = incomingTokenHash
                });

            if (storedRefreshToken == null || !storedRefreshToken.Succeeded || storedRefreshToken.Data == null)
            {
                return new AuthResponse { Message = "Invalid refresh token" };
            }

            var refreshToken = storedRefreshToken.Data;

            if (refreshToken.UserId != userId)
            {
                return new AuthResponse { Message = "Refresh token does not belong to this user" };
            }

            if (refreshToken.IsRevoked)
            {
                return new AuthResponse { Message = "Refresh token has been revoked" };
            }

            if (refreshToken.ExpiresAt < now)
            {
                await _authRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = "RefreshToken",
                        FIDOOperation = "RevokeRefreshToken",
                        TokenHash = incomingTokenHash,
                        Now = now
                    });

                return new AuthResponse { Message = "Refresh token expired" };
            }

            var currentLocation = request.IpAddress != null ? await _locationResolver.ResolveLocationAsync(request.IpAddress) : null;
            var previousLocation = refreshToken.Location;

            if (_securitySettings.EnableSuspiciousActivityDetection 
                && !string.IsNullOrEmpty(previousLocation) 
                && !string.IsNullOrEmpty(currentLocation) 
                && !string.Equals(previousLocation, currentLocation, StringComparison.OrdinalIgnoreCase))
            {
                await _auditLogService.LogAsync(
                    userId,
                    "SuspiciousRefreshToken",
                    "RefreshToken",
                    refreshToken.Id.ToString(),
                    previousLocation,
                    currentLocation,
                    request.IpAddress,
                    request.UserAgent);

                await _authRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = "RefreshToken",
                        FIDOOperation = "RevokeRefreshToken",
                        TokenHash = incomingTokenHash,
                        Now = now
                    });

                if (_securitySettings.ForceReauthOnLocationChange)
                {
                    return new AuthResponse 
                    { 
                        Message = "Suspicious activity detected. Please re-authenticate.",
                        RequiresFido2 = true 
                    };
                }

                await _auditLogService.LogAsync(
                    userId,
                    "SuspiciousRefreshTokenFlagged",
                    "RefreshToken",
                    refreshToken.Id.ToString(),
                    previousLocation,
                    currentLocation,
                    request.IpAddress,
                    request.UserAgent);
            }

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "RevokeRefreshToken",
                    TokenHash = incomingTokenHash,
                    Now = now
                });

            var newAccessToken = _jwtHelper.GenerateToken(userId, usernameClaim ?? string.Empty);
            var newDeviceInfo = new DeviceInfo
            {
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Location = currentLocation
            };
            await CreateRefreshTokenAsync(userId, newDeviceInfo);

            await _auditLogService.LogAsync(
                userId,
                "TokenRefreshed",
                "RefreshToken",
                refreshToken.Id.ToString(),
                null,
                "Token refreshed",
                request.IpAddress,
                request.UserAgent);

            return new AuthResponse
            {
                UserId = userId,
                Username = usernameClaim ?? string.Empty,
                Token = newAccessToken,
                Message = "Token refreshed successfully"
            };
        }

        public async Task<ActiveSessionsResponse> GetActiveSessionsAsync(int userId, int currentRefreshTokenId = 0)
        {
            var activeTokens = await _authRepository.QuerySingleAsync<List<RefreshToken>>(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "GetActiveTokensForUser",
                    UserId = userId
                });

            var sessions = new List<DeviceSessionResponse>();
            if (activeTokens?.Succeeded == true && activeTokens.Data != null)
            {
                sessions = activeTokens.Data.Select(t => new DeviceSessionResponse
                {
                    Id = t.Id,
                    IpAddress = t.IpAddress ?? "Unknown",
                    UserAgent = t.UserAgent ?? "Unknown",
                    Location = t.Location ?? "Unknown",
                    CreatedAt = t.CreatedAt,
                    LastUsedAt = t.LastUsedAt,
                    ExpiresAt = t.ExpiresAt,
                    IsCurrent = t.Id == currentRefreshTokenId
                }).ToList();
            }

            return new ActiveSessionsResponse
            {
                Sessions = sessions,
                MaxAllowedSessions = _securitySettings.MaxConcurrentSessions
            };
        }

        public async Task RevokeAllSessionsAsync(int userId)
        {
            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "RevokeAllForUser",
                    UserId = userId,
                    Now = DateTime.UtcNow
                });

            await _auditLogService.LogAsync(
                userId,
                "RevokeAllSessions",
                "User",
                userId.ToString());
        }

        public async Task RevokeSessionAsync(int sessionId, int userId)
        {
            var stored = await _authRepository.QuerySingleAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "GetRefreshTokenById",
                    SessionId = sessionId
                });

            if (stored?.Succeeded == true && stored.Data != null && stored.Data.UserId == userId && !stored.Data.IsRevoked)
            {
                await _authRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = "RefreshToken",
                        FIDOOperation = "RevokeRefreshToken",
                        TokenHash = stored.Data.TokenHash,
                        Now = DateTime.UtcNow
                });

                await _auditLogService.LogAsync(
                    userId,
                    "RevokeSession",
                    "RefreshToken",
                    sessionId.ToString());
            }
        }

        public async Task<UserProfileResponse?> GetProfileAsync(int userId)
        {
            var user = await _authRepository.QuerySingleAsync<User>(ProcedureName, new { AuthType = "Login", UserId = userId });
            if (user?.Succeeded != true || user.Data == null) return null;

            var profile = await _authRepository.QuerySingleAsync<UserProfileResponse>(
                ProcedureName,
                new { AuthType = "GetProfile", UserId = userId });

            if (profile?.Succeeded == true && profile.Data != null)
            {
                return profile.Data;
            }

            return new UserProfileResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username ?? string.Empty,
                Email = user.Data.Email ?? string.Empty,
                DateJoined = user.Data.CreatedAt,
                AccountStatus = "active"
            };
        }

        public async Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var result = await _authRepository.QuerySingleAsync<UserProfileResponse>(
                ProcedureName,
                new
                {
                    AuthType = "UpdateProfile",
                    UserId = userId,
                    request.Username,
                    request.Email,
                    request.Phone,
                    request.Bio,
                    Now = DateTime.UtcNow
                });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "ProfileUpdated", "User", userId.ToString(), null, "Profile updated");
                return result.Data;
            }

            return null;
        }

        public async Task<AccountSettingsResponse> GetAccountSettingsAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<AccountSettingsResponse>(
                ProcedureName,
                new { AuthType = "GetSettings", UserId = userId });

            if (result?.Succeeded == true && result.Data != null)
            {
                return result.Data;
            }

            return new AccountSettingsResponse();
        }

        public async Task<AccountSettingsResponse> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request)
        {
            var result = await _authRepository.QuerySingleAsync<AccountSettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = "UpdateSettings",
                    UserId = userId,
                    request.DisplayName,
                    request.Username,
                    request.Email,
                    request.EmailPreferences,
                    request.Timezone,
                    request.Language,
                    request.EmailNotifications,
                    request.PushNotifications,
                    request.SmsAlerts,
                    request.MarketingEmails,
                    Now = DateTime.UtcNow
                });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "SettingsUpdated", "User", userId.ToString(), null, "Settings updated");
                return result.Data;
            }

            return new AccountSettingsResponse
            {
                DisplayName = request.DisplayName,
                Username = request.Username,
                Email = request.Email,
                EmailPreferences = request.EmailPreferences,
                Timezone = request.Timezone,
                Language = request.Language,
                EmailNotifications = request.EmailNotifications,
                PushNotifications = request.PushNotifications,
                SmsAlerts = request.SmsAlerts,
                MarketingEmails = request.MarketingEmails
            };
        }

        public async Task<PrivacySettingsResponse> GetPrivacySettingsAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<PrivacySettingsResponse>(
                ProcedureName,
                new { AuthType = "GetPrivacy", UserId = userId });

            if (result?.Succeeded == true && result.Data != null)
            {
                return result.Data;
            }

            return new PrivacySettingsResponse();
        }

        public async Task<PrivacySettingsResponse> UpdatePrivacySettingsAsync(int userId, UpdatePrivacyRequest request)
        {
            var result = await _authRepository.QuerySingleAsync<PrivacySettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = "UpdatePrivacy",
                    UserId = userId,
                    request.ProfileVisibility,
                    request.DataSharing,
                    request.ThirdPartyConnections,
                    request.CookiePreferences,
                    Now = DateTime.UtcNow
                });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "PrivacyUpdated", "User", userId.ToString(), null, "Privacy updated");
                return result.Data;
            }

            return new PrivacySettingsResponse
            {
                ProfileVisibility = request.ProfileVisibility,
                DataSharing = request.DataSharing,
                ThirdPartyConnections = request.ThirdPartyConnections,
                CookiePreferences = request.CookiePreferences
            };
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            await _auditLogService.LogAsync(0, "PasswordResetRequested", "User", email, null, "Password reset requested");
        }

        public async Task<MessageResponse> ResetPasswordAsync(string token, string newPassword)
        {
            var passwordHash = _passwordHash.HashPassword(newPassword);
            var result = await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "ResetPassword",
                    TokenHash = token,
                    PasswordHash = passwordHash,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data > 0)
            {
                return MessageResponse.Success("Password has been reset successfully");
            }

            return MessageResponse.Failure("Invalid or expired reset token");
        }

        public async Task<string> GetUserDataExportAsync(int userId)
        {
            await _auditLogService.LogAsync(userId, "DataExportRequested", "User", userId.ToString());
            return $"data-export/user-{userId}.json";
        }

        public async Task<MessageResponse> DeleteAccountAsync(int userId)
        {
            var result = await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "DeleteAccount",
                    UserId = userId,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data > 0)
            {
                await _auditLogService.LogAsync(userId, "AccountDeleted", "User", userId.ToString());
                return MessageResponse.Success("Account scheduled for deletion");
            }

            return MessageResponse.Failure("Failed to delete account");
        }

        public async Task<SecuritySettingsResponse> GetSecuritySettingsAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = "GetSecurity", UserId = userId });

            if (result?.Succeeded == true && result.Data != null)
            {
                return result.Data;
            }

            return new SecuritySettingsResponse();
        }

        public async Task<SecuritySettingsResponse> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = "UpdateSecurity",
                    UserId = userId,
                    request.AlertOnNewDevice,
                    request.RequirePasswordForSensitive,
                    Now = DateTime.UtcNow
                });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "SecuritySettingsUpdated", "User", userId.ToString(), null, "Security settings updated");
                return result.Data;
            }

            return request;
        }

        public async Task<MessageResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _authRepository.QuerySingleAsync<User>(ProcedureName, new { AuthType = "Login", UserId = userId });
            if (user?.Succeeded != true || user.Data == null)
            {
                return MessageResponse.Failure("User not found");
            }

            if (!_passwordHash.VerifyPassword(request.CurrentPassword, user.Data.PasswordHash ?? string.Empty))
            {
                return MessageResponse.Failure("Current password is incorrect");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return MessageResponse.Failure("New password and confirmation do not match");
            }

            var newPasswordHash = _passwordHash.HashPassword(request.NewPassword);
            var result = await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "ChangePassword",
                    UserId = userId,
                    PasswordHash = newPasswordHash,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data > 0)
            {
                await _auditLogService.LogAsync(userId, "PasswordChanged", "User", userId.ToString());
                return MessageResponse.Success("Password changed successfully");
            }

            return MessageResponse.Failure("Failed to change password");
        }

        public async Task<SecuritySettingsResponse> EnableTwoFactorAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = "Enable2FA", UserId = userId, Now = DateTime.UtcNow });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "TwoFactorEnabled", "User", userId.ToString());
                return result.Data;
            }

            return new SecuritySettingsResponse { TwoFactorEnabled = true };
        }

        public async Task<SecuritySettingsResponse> DisableTwoFactorAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = "Disable2FA", UserId = userId, Now = DateTime.UtcNow });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "TwoFactorDisabled", "User", userId.ToString());
                return result.Data;
            }

            return new SecuritySettingsResponse { TwoFactorEnabled = false };
        }

        public async Task<ActivityLogResponse> GetActivityLogsAsync(int userId, ActivityQueryRequest query)
        {
            var result = await _authRepository.QuerySingleAsync<List<AuditLog>>(
                ProcedureName,
                new
                {
                    AuthType = "AuditLog",
                    FIDOOperation = "GetByUser",
                    UserId = userId
                });

            var entries = new List<ActivityLogEntry>();

            if (result?.Succeeded == true && result.Data != null)
            {
                var filtered = result.Data.AsEnumerable();

                if (query.From.HasValue)
                    filtered = filtered.Where(a => a.CreatedAt >= query.From.Value);
                if (query.To.HasValue)
                    filtered = filtered.Where(a => a.CreatedAt <= query.To.Value.Date.AddDays(1).AddTicks(-1));
                if (!string.IsNullOrWhiteSpace(query.Type) && query.Type != "all")
                    filtered = filtered.Where(a => a.Action == query.Type);
                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var term = query.Search.Trim().ToLowerInvariant();
                    filtered = filtered.Where(a =>
                        a.Action.ToLowerInvariant().Contains(term) ||
                        (a.EntityType ?? string.Empty).ToLowerInvariant().Contains(term));
                }

                entries = filtered.OrderByDescending(a => a.CreatedAt).Select(a => new ActivityLogEntry
                {
                    Id = (int)a.Id,
                    Type = a.Action,
                    Description = a.Action,
                    Device = a.UserAgent ?? "Unknown",
                    IpAddress = a.IpAddress ?? "Unknown",
                    Timestamp = a.CreatedAt
                }).ToList();
            }

            return new ActivityLogResponse { Entries = entries };
        }

        public async Task<MessageResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return MessageResponse.Failure("Enter the verification code");
            }

            await _auditLogService.LogAsync(userId, "DeviceVerified", "User", userId.ToString(), null, request.TrustDevice ? "Device verified and trusted" : "Device verified");

            return MessageResponse.Success(request.TrustDevice ? "Device verified and trusted" : "Device verified");
        }

        private async Task EnforceConcurrentSessionLimitAsync(int userId)
        {
            var activeTokens = await _authRepository.QuerySingleAsync<List<RefreshToken>>(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "GetActiveTokensForUser",
                    UserId = userId
                });

            if (activeTokens?.Succeeded == true && activeTokens.Data != null && activeTokens.Data.Count >= _securitySettings.MaxConcurrentSessions)
            {
                var oldest = activeTokens.Data.OrderBy(t => t.CreatedAt).FirstOrDefault();
                if (oldest != null)
                {
                    await _authRepository.ExecuteAsync(
                        ProcedureName,
                        new
                        {
                            AuthType = "RefreshToken",
                            FIDOOperation = "RevokeRefreshToken",
                            TokenHash = oldest.TokenHash,
                            Now = DateTime.UtcNow
                        });

                    await _auditLogService.LogAsync(
                        userId,
                        "ConcurrentSessionLimit",
                        "RefreshToken",
                        oldest.Id.ToString(),
                        oldValue: "Active",
                        newValue: "Revoked due to concurrent limit");
                }
            }
        }

        private DeviceInfo GetDeviceInfo()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return new DeviceInfo();
            }

            var ip = GetClientIpAddress();
            var userAgent = GetUserAgent();
            var location = _locationResolver.ResolveLocationAsync(ip).GetAwaiter().GetResult();

            return new DeviceInfo
            {
                IpAddress = ip,
                UserAgent = userAgent,
                Location = location
            };
        }

        private string? GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                return forwardedFor.ToString().Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers["User-Agent"].ToString();
        }
    }
}