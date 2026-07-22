using System.Transactions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Shared.Core.Models.Entities;
using Shared.Core.Models.Account;
using Shared.Core.Models.Auth;
using Shared.Core.Models.Security;
using Shared.Core.Models.Rbac;
using Shared.Data.Repository.Interface;
using Shared.Core.Wrapper;
using Auth.API.Config;
using Auth.API.Configuration;
using Auth.API.Utility.Jwt;
using Auth.API.Utility.PasswordHash;
using Auth.API.Utility.TokenHash;
using Auth.API.Service.Interface.Auth;
using Auth.API.Service.Interface.Rbac;
using Auth.API.Service.Interface.Security;
using Auth.UI.Models.Auth;

namespace Auth.API.Service.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _authRepository;
        private readonly IPasswordHash _passwordHash;
        private readonly IJwtHelper _jwtHelper;
        private readonly IFido2Service _fido2Service;
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private readonly IOtpService _otpService;
        private readonly IAuditLogService _auditLogService;
        private readonly ILocationResolver _locationResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly SecuritySettings _securitySettings;
        private readonly ApiSettings _apiSettings;

        private const string ProcedureName = DbConstants.Procedures.Users;

        public AuthService(IGenericRepository<User> authRepository, IPasswordHash passwordHash, IJwtHelper jwtHelper, IFido2Service fido2Service, IDapperRepository dapperRepository, IUserRoleService userRoleService, IRoleService roleService, IOtpService otpService, IAuditLogService auditLogService, ILocationResolver locationResolver, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IOptions<SecuritySettings> securitySettings, IOptions<ApiSettings> apiSettings)
        {
            _authRepository = authRepository;
            _passwordHash = passwordHash;
            _jwtHelper = jwtHelper;
            _fido2Service = fido2Service;
            _dapperRepository = dapperRepository;
            _userRoleService = userRoleService;
            _roleService = roleService;
            _otpService = otpService;
            _auditLogService = auditLogService;
            _locationResolver = locationResolver;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _securitySettings = securitySettings.Value;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var passwordHash = _passwordHash.HashPassword(request.Password);

                using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
                var param = new
                {
                    AuthType = DbConstants.AuthTypes.Register,
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash
                };

                var userIdResult = await _authRepository.QuerySingleAsync<UserIdResult>(
                    ProcedureName,
                    param
                );

                if (userIdResult == null || !userIdResult.Succeeded || userIdResult.Data == null)
                {
                    return Response<AuthResponse>.Fail("Registration failed");
                }

                var token = _jwtHelper.GenerateToken(userIdResult.Data.UserId, request.Username);
                var deviceInfo = await GetDeviceInfoAsync();
                var refreshToken = await CreateRefreshTokenAsync(userIdResult.Data.UserId, deviceInfo);
                await AssignDefaultRoleIfMissingAsync(userIdResult.Data.UserId);

                var userWithRoles = (await _userRoleService.GetUserWithRolesAndPermissionsAsync(userIdResult.Data.UserId)).Data;

                await _auditLogService.LogAsync(
                    userIdResult.Data.UserId,
                    "UserRegistered",
                    "User",
                    userIdResult.Data.UserId.ToString(),
                    null,
                    request.Username,
                    deviceInfo.IpAddress,
                    deviceInfo.UserAgent);

                scope.Complete();

                return Response<AuthResponse>.Success(new AuthResponse
                {
                    UserId = userIdResult.Data.UserId,
                    Username = request.Username,
                    Email = request.Email,
                    Token = token,
                    RefreshToken = refreshToken,
                    Message = "Registered successfully",
                    RequiresFido2Registration = true,
                    Role = userWithRoles?.Role,
                    Permissions = userWithRoles?.Permissions ?? new List<string>()
                });
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail("Registration failed: " + ex.Message);
            }
        }

        public async Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress = null, string? userAgent = null)
        {
            var result = await _authRepository.QuerySingleAsync<UserIdResult>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Login, Username = request.Username });

            if (result == null || !result.Succeeded || result.Data == null || result.Data.UserId <= 0)
            {
                return Response<AuthResponse>.Fail("Invalid username or password");
            }

            var user = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Login, UserId = result.Data.UserId });

            if (user == null || !user.Succeeded || user.Data == null || string.IsNullOrEmpty(user.Data.PasswordHash))
            {
                return Response<AuthResponse>.Fail("Invalid username or password");
            }

            if (!_passwordHash.VerifyPassword(request.Password, user.Data.PasswordHash))
            {
                return Response<AuthResponse>.Fail("Invalid username or password");
            }

            bool hasFido2 = await HasFido2CredentialsAsync(user.Data.Id);

            if (hasFido2)
            {
                return Response<AuthResponse>.Success(new AuthResponse
                {
                    UserId = user.Data.Id,
                    Username = user.Data.Username,
                    Email = user.Data.Email,
                    Message = "FIDO2 verification required",
                    RequiresFido2 = true
                });
            }

            var token = _jwtHelper.GenerateToken(user.Data.Id, user.Data.Username);
            var deviceInfo = new DeviceInfo
            {
                IpAddress = ipAddress ?? GetClientIpAddress(),
                UserAgent = userAgent ?? GetUserAgent()
            };

            deviceInfo.Location = await _locationResolver.ResolveLocationAsync(deviceInfo.IpAddress);

            await AssignDefaultRoleIfMissingAsync(user.Data.Id);
            await EnforceConcurrentSessionLimitAsync(user.Data.Id);
            var refreshToken = await CreateRefreshTokenAsync(user.Data.Id, deviceInfo);

            await _auditLogService.LogAsync(
                user.Data.Id,
                "UserLoggedIn",
                "User",
                user.Data.Id.ToString(),
                null,
                "Login successful",
                deviceInfo.IpAddress,
                deviceInfo.UserAgent);

            var userWithRoles = (await _userRoleService.GetUserWithRolesAndPermissionsAsync(user.Data.Id)).Data;

            return Response<AuthResponse>.Success(new AuthResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username,
                Email = user.Data.Email,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Login successful",
                RequiresFido2 = false,
                RequiresFido2Registration = !hasFido2,
                Role = userWithRoles?.Role,
                Permissions = userWithRoles?.Permissions ?? new List<string>()
            });
        }

        public async Task<IResponse<User?>> GetUserByIdAsync(int userId)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Login, UserId = userId });
            if (userResult.Succeeded)
            {
                return Response<User?>.Success(userResult.Data);
            }
            else
            {
                return Response<User?>.Fail(userResult.Messages ?? "User not found");
            }
        }

        public async Task<IResponse<User?>> GetUserByEmailAsync(string email)
        {
            var userResult = await _authRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Login, Email = email });

            if (userResult.Succeeded)
            {
                return Response<User?>.Success(userResult.Data);
            }
            else
            {
                return Response<User?>.Fail(userResult.Messages ?? "User not found");
            }
        }

        public async Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request)
        {
            try
            {
                var result = await _otpService.RequestOtpAsync(request);
                if (result.Succeeded)
                {
                    return Response<OtpResponse>.Success(result.Data);
                }
                else
                {
                    return Response<OtpResponse>.Fail(result.Messages);
                }
            }
            catch (Exception ex)
            {
                return Response<OtpResponse>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request)
        {
            try
            {
                var result = await _otpService.VerifyOtpAsync(request);
                if (result.Succeeded)
                {
                    return Response<AuthResponse>.Success(result.Data);
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages);
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            try
            {
                return Response<Fido2ChallengeResponse>.Success((await _fido2Service.RequestAttestationOptionsAsync(request.UserId, request.Username, request.Origin ?? string.Empty)).Data);
            }
            catch (Exception ex)
            {
                return Response<Fido2ChallengeResponse>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            try
            {
                return Response<Fido2VerifyResponse>.Success((await _fido2Service.RegisterCredentialAsync(request, request.Origin ?? string.Empty)).Data);
            }
            catch (Exception ex)
            {
                return Response<Fido2VerifyResponse>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(Fido2ChallengeRequest request)
        {
            try
            {
                return Response<Fido2ChallengeResponse>.Success((await _fido2Service.CreateChallengeAsync(request.UserId, request.Origin ?? string.Empty)).Data);
            }
            catch (Exception ex)
            {
                return Response<Fido2ChallengeResponse>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            try
            {
                return Response<Fido2VerifyResponse>.Success((await _fido2Service.VerifyAssertionAsync(request, request.Origin ?? string.Empty)).Data);
            }
            catch (Exception ex)
            {
                return Response<Fido2VerifyResponse>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
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
                return Response<AuthResponse>.Fail("Invalid access token");
            }

            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Response<AuthResponse>.Fail("Invalid access token claims");
            }

            var incomingTokenHash = TokenHasher.HashToken(request.RefreshToken);

            var storedRefreshToken = await _authRepository.QuerySingleAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.GetRefreshToken,
                    TokenHash = incomingTokenHash
                });

            if (storedRefreshToken == null || !storedRefreshToken.Succeeded || storedRefreshToken.Data == null)
            {
                return Response<AuthResponse>.Fail("Invalid refresh token");
            }

            var refreshToken = storedRefreshToken.Data;

            if (refreshToken.UserId != userId)
            {
                return Response<AuthResponse>.Fail("Refresh token does not belong to this user");
            }

            if (refreshToken.IsRevoked)
            {
                await RevokeAllSessionsAsync(userId);
                await _auditLogService.LogAsync(
                    userId,
                    "TokenReuseDetected",
                    "RefreshToken",
                    refreshToken.Id.ToString(),
                    null,
                    "Revoked refresh token reused; all sessions invalidated");

                return Response<AuthResponse>.Fail("Session invalidated due to suspicious activity");
            }

            if (refreshToken.ExpiresAt < now)
            {
                await _authRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = DbConstants.AuthTypes.RefreshToken,
                        FIDOOperation = DbConstants.FidoOperations.RevokeRefreshToken,
                        TokenHash = incomingTokenHash,
                        Now = now
                    });

                return Response<AuthResponse>.Fail("Refresh token expired");
            }

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.RevokeRefreshToken,
                    TokenHash = incomingTokenHash,
                    Now = now
                });

            var newAccessToken = _jwtHelper.GenerateToken(userId, usernameClaim ?? string.Empty);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            var newDeviceInfo = new DeviceInfo
            {
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent
            };
            var currentLocation = await _locationResolver.ResolveLocationAsync(newDeviceInfo.IpAddress);
            newDeviceInfo.Location = currentLocation;

            await EnforceConcurrentSessionLimitAsync(userId);
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

            return Response<AuthResponse>.Success(new AuthResponse
            {
                UserId = userId,
                Username = usernameClaim ?? string.Empty,
                Token = newAccessToken,
                Message = "Token refreshed successfully"
            });
        }

        public async Task<IResponse<ActiveSessionsResponse>> GetActiveSessionsAsync(int userId, int currentRefreshTokenId = 0)
        {
            var activeTokens = await _authRepository.QueryAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.GetActiveTokensForUser,
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

            return Response<ActiveSessionsResponse>.Success(new ActiveSessionsResponse
            {
                Sessions = sessions,
                MaxAllowedSessions = _securitySettings.MaxConcurrentSessions
            });
        }

        public async Task<IResponse> RevokeAllSessionsAsync(int userId)
        {
            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.RevokeAllForUser,
                    UserId = userId,
                    Now = DateTime.UtcNow
                });

            await _auditLogService.LogAsync(userId, "RevokeAllSessions", "User", userId.ToString());

            return Response.Success();
        }

        public async Task<IResponse> RevokeSessionAsync(int sessionId, int userId)
        {
            var stored = await _authRepository.QuerySingleAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.GetRefreshTokenById,
                    SessionId = sessionId
                });

            if (stored?.Succeeded == true && stored.Data != null && stored.Data.UserId == userId && !stored.Data.IsRevoked)
            {
                await _authRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = DbConstants.AuthTypes.RefreshToken,
                        FIDOOperation = DbConstants.FidoOperations.RevokeRefreshToken,
                        TokenHash = stored.Data.TokenHash,
                        Now = DateTime.UtcNow
                    });

                await _auditLogService.LogAsync(userId, "RevokeSession", "RefreshToken", sessionId.ToString());
            }

            return Response.Success();
        }

        public async Task<IResponse<UserProfileResponse?>> GetProfileAsync(int userId)
        {
            var user = await _authRepository.QuerySingleAsync<User>(ProcedureName, new { AuthType = DbConstants.AuthTypes.Login, UserId = userId });
            if (user?.Succeeded != true || user.Data == null) return Response<UserProfileResponse?>.Fail("User not found");

            var profile = await _authRepository.QuerySingleAsync<UserProfileResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.GetProfile, UserId = userId });

            if (profile?.Succeeded == true && profile.Data != null)
            {
                return Response<UserProfileResponse?>.Success(profile.Data);
            }

            return Response<UserProfileResponse?>.Success(new UserProfileResponse
            {
                UserId = user.Data.Id,
                Username = user.Data.Username ?? string.Empty,
                Email = user.Data.Email ?? string.Empty,
                DateJoined = user.Data.CreatedAt,
                AccountStatus = "active"
            });
        }

        public async Task<IResponse<UserProfileResponse?>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var result = await _authRepository.QuerySingleAsync<UserProfileResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdateProfile,
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
                return Response<UserProfileResponse?>.Success(result.Data);
            }

            return Response<UserProfileResponse?>.Fail("Failed to update profile");
        }

        public async Task<IResponse<AccountSettingsResponse>> GetAccountSettingsAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<AccountSettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.GetSettings, UserId = userId });

            if (result?.Succeeded == true && result.Data != null)
            {
                return Response<AccountSettingsResponse>.Success(result.Data);
            }

            return Response<AccountSettingsResponse>.Success(new AccountSettingsResponse());
        }

        public async Task<IResponse<AccountSettingsResponse>> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request)
        {
            var result = await _authRepository.QuerySingleAsync<AccountSettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdateSettings,
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
                return Response<AccountSettingsResponse>.Success(result.Data);
            }

            return Response<AccountSettingsResponse>.Success(new AccountSettingsResponse
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
            });
        }

        public async Task<IResponse<PrivacySettingsResponse>> GetPrivacySettingsAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<PrivacySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.GetPrivacy, UserId = userId });

            if (result?.Succeeded == true && result.Data != null)
            {
                return Response<PrivacySettingsResponse>.Success(result.Data);
            }

            return Response<PrivacySettingsResponse>.Success(new PrivacySettingsResponse());
        }

        public async Task<IResponse<PrivacySettingsResponse>> UpdatePrivacySettingsAsync(int userId, UpdatePrivacyRequest request)
        {
            var result = await _authRepository.QuerySingleAsync<PrivacySettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdatePrivacy,
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
                return Response<PrivacySettingsResponse>.Success(result.Data);
            }

            return Response<PrivacySettingsResponse>.Success(new PrivacySettingsResponse
            {
                ProfileVisibility = request.ProfileVisibility,
                DataSharing = request.DataSharing,
                ThirdPartyConnections = request.ThirdPartyConnections,
                CookiePreferences = request.CookiePreferences
            });
        }

        public async Task<IResponse> RequestPasswordResetAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null || user.Data == null) return Response.Success();

            var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var expiresAt = DateTime.UtcNow.AddHours(1);

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.ResetPassword,
                    FIDOOperation = "CreateResetToken",
                    Email = email,
                    TokenHash = token,
                    ExpiresAt = expiresAt,
                    Now = DateTime.UtcNow
                });

            var frontendBaseUrl = _apiSettings.FrontendBaseUrl;
            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                frontendBaseUrl = _apiSettings.BaseUrl;
            }

            var resetLink = $"{frontendBaseUrl.TrimEnd('/')}/reset-password?token={token}&email={Uri.EscapeDataString(email)}";
            await _emailService.SendAsync(
                email,
                DbConstants.Email.PasswordResetSubject,
                string.Format(DbConstants.Email.PasswordResetBodyTemplate, resetLink));

            await _auditLogService.LogAsync(user.Data.Id, "PasswordResetRequested", "User", user.Data.Id.ToString(), null, "Password reset requested");

            return Response.Success();
        }

        public async Task<IResponse> ResetPasswordAsync(string token, string newPassword)
        {
            var passwordHash = _passwordHash.HashPassword(newPassword);
            var result = await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.ResetPassword,
                    TokenHash = token,
                    PasswordHash = passwordHash,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data > 0)
            {
                return Response.Success("Password has been reset successfully");
            }

            return Response.Fail("Invalid or expired reset token");
        }

        public async Task<IResponse<string>> GetUserDataExportAsync(int userId)
        {
            await _auditLogService.LogAsync(userId, "DataExportRequested", "User", userId.ToString());
            return Response<string>.Success($"data-export/user-{userId}.json");
        }

        public async Task<IResponse> DeleteAccountAsync(int userId)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

            var result = await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.DeleteAccount,
                    UserId = userId,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data > 0)
            {
                await _auditLogService.LogAsync(userId, "AccountDeleted", "User", userId.ToString());
                scope.Complete();
                return Response.Success("Account scheduled for deletion");
            }

            return Response.Fail("Failed to delete account");
        }

        public async Task<IResponse<SecuritySettingsResponse>> GetSecuritySettingsAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.GetSecurity, UserId = userId });

            if (result?.Succeeded == true && result.Data != null)
            {
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(new SecuritySettingsResponse());
        }

        public async Task<IResponse<SecuritySettingsResponse>> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdateSecurity,
                    UserId = userId,
                    request.AlertOnNewDevice,
                    request.RequirePasswordForSensitive,
                    Now = DateTime.UtcNow
                });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "SecuritySettingsUpdated", "User", userId.ToString(), null, "Security settings updated");
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(request);
        }

        public async Task<IResponse<SecuritySettingsResponse>> EnableTwoFactorAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Enable2Fa, UserId = userId, Now = DateTime.UtcNow });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "TwoFactorEnabled", "User", userId.ToString());
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(new SecuritySettingsResponse { TwoFactorEnabled = true });
        }

        public async Task<IResponse<SecuritySettingsResponse>> DisableTwoFactorAsync(int userId)
        {
            var result = await _authRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Disable2Fa, UserId = userId, Now = DateTime.UtcNow });

            if (result?.Succeeded == true && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "TwoFactorDisabled", "User", userId.ToString());
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(new SecuritySettingsResponse { TwoFactorEnabled = false });
        }

        public async Task<IResponse<ActivityLogResponse>> GetActivityLogsAsync(int userId, ActivityQueryRequest query)
        {
            var result = await _authRepository.QueryAsync<AuditLog>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.AuditLog,
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

            return Response<ActivityLogResponse>.Success(new ActivityLogResponse { Entries = entries });
        }

        public async Task<IResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Response.Fail("Enter the verification code");
            }

            await _auditLogService.LogAsync(userId, "DeviceVerified", "User", userId.ToString(), null, request.TrustDevice ? "Device verified and trusted" : "Device verified");

            return Response.Success(request.TrustDevice ? "Device verified and trusted" : "Device verified");
        }

        public async Task<IResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _authRepository.QuerySingleAsync<User>(ProcedureName, new { AuthType = DbConstants.AuthTypes.Login, UserId = userId });
            if (user?.Succeeded != true || user.Data == null)
            {
                return Response.Fail("User not found");
            }

            if (!_passwordHash.VerifyPassword(request.CurrentPassword, user.Data.PasswordHash ?? string.Empty))
            {
                return Response.Fail("Current password is incorrect");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return Response.Fail("New password and confirmation do not match");
            }

            var newPasswordHash = _passwordHash.HashPassword(request.NewPassword);
            var result = await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.ChangePassword,
                    UserId = userId,
                    PasswordHash = newPasswordHash,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data > 0)
            {
                await _auditLogService.LogAsync(userId, "PasswordChanged", "User", userId.ToString());
                return Response.Success("Password changed successfully");
            }

            return Response.Fail("Failed to change password");
        }

        private async Task<bool> HasFido2CredentialsAsync(int userId)
        {
            var result = await _authRepository.QueryAsync<UserCredential>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetCredentialsByUserId, UserId = userId });

            return result.Succeeded && result.Data != null && result.Data.Any();
        }

        private async Task AssignDefaultRoleIfMissingAsync(int userId)
        {
            var userRoles = (await _userRoleService.GetUserRoleNamesAsync(userId)).Data ?? new List<string>();
            if (!userRoles.Any())
            {
                var user = await GetUserByIdAsync(userId);
                if (user != null && user.Data != null && !string.IsNullOrEmpty(user.Data.Username))
                {
                    var roleName = user.Data.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";
                    var role = (await _roleService.GetRoleByNameAsync(roleName)).Data;
                    if (role == null)
                    {
                        role = (await _roleService.CreateRoleAsync(roleName, $"Default {roleName} role")).Data;
                    }

                    if (role != null)
                    {
                        await _userRoleService.AssignRoleToUserAsync(userId, role.Id);
                    }
                }
            }
        }

        private async Task<string> CreateRefreshTokenAsync(int userId, DeviceInfo deviceInfo)
        {
            var now = DateTime.UtcNow;
            var rawToken = _jwtHelper.GenerateRefreshToken();
            var tokenHash = TokenHasher.HashToken(rawToken);
            var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            await _authRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.CreateRefreshToken,
                    UserId = userId,
                    TokenHash = tokenHash,
                    ExpiresAt = now.AddDays(refreshExpiryDays),
                    Now = now,
                    IpAddress = deviceInfo.IpAddress,
                    UserAgent = deviceInfo.UserAgent,
                    Location = deviceInfo.Location
                });

            return rawToken;
        }

        private async Task EnforceConcurrentSessionLimitAsync(int userId)
        {
            var activeTokens = await _authRepository.QueryAsync<RefreshToken>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.GetActiveTokensForUser,
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
                            AuthType = DbConstants.AuthTypes.RefreshToken,
                            FIDOOperation = DbConstants.FidoOperations.RevokeRefreshToken,
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

        private async Task<DeviceInfo> GetDeviceInfoAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return new DeviceInfo();
            }

            var ip = GetClientIpAddress();
            var userAgent = GetUserAgent();
            var location = await _locationResolver.ResolveLocationAsync(ip);

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
