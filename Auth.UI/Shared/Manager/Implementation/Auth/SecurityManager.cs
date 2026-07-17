using Shared.Wrapper;
using Auth.UI.Shared.Model.Security;
using Auth.UI.Shared.Utility;
using UI.Shared.Manager.Interface.Auth;
using UI.Shared.Manager.Interface.Http;
using UI.Shared.Manager.Routes;

namespace UI.Shared.Manager.Implementation.Auth
{
    public class SecurityManager : ISecurityManager
    {
        private readonly IHttpServices _httpService;
        private readonly ITokenStore _tokenStore;

        public SecurityManager(IHttpServices httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
        }

        public async Task<IResponse<SecuritySettings>> GetSecurityAsync()
        {
            try
            {
                var result = await _httpService.GetAsync<SecuritySettings>(SecurityRoute.Settings);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<SecuritySettings>.Success(result.Data, "Security settings retrieved");
                }
                else
                {
                    return Response<SecuritySettings>.Fail(result.Messages ?? "Failed to retrieve security settings");
                }
            }
            catch (Exception ex)
            {
                return Response<SecuritySettings>.Fail($"An error occurred while retrieving the security settings: {ex.Message}");
            }
        }

        public async Task<IResponse<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<SecuritySettings>(SecurityRoute.Settings, settings);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<SecuritySettings>.Success(result.Data, "Security settings updated");
                }
                else
                {
                    return Response<SecuritySettings>.Fail(result.Messages ?? "Failed to update security settings");
                }
            }
            catch (Exception ex)
            {
                return Response<SecuritySettings>.Fail($"An error occurred while updating the security settings: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<ActionResponse>(SecurityRoute.ChangePassword, request);
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Password changed");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to change password");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while changing the password: {ex.Message}");
            }
        }

        public async Task<IResponse<SecuritySettings>> EnableTwoFactorAsync()
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<SecuritySettings>(SecurityRoute.Enable2FA, null!);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<SecuritySettings>.Success(result.Data, "Two-factor authentication enabled");
                }
                else
                {
                    return Response<SecuritySettings>.Fail(result.Messages ?? "Failed to enable two-factor authentication");
                }
            }
            catch (Exception ex)
            {
                return Response<SecuritySettings>.Fail($"An error occurred while enabling two-factor authentication: {ex.Message}");
            }
        }

        public async Task<IResponse<SecuritySettings>> DisableTwoFactorAsync()
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<SecuritySettings>(SecurityRoute.Disable2FA, null!);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<SecuritySettings>.Success(result.Data, "Two-factor authentication disabled");
                }
                else
                {
                    return Response<SecuritySettings>.Fail(result.Messages ?? "Failed to disable two-factor authentication");
                }
            }
            catch (Exception ex)
            {
                return Response<SecuritySettings>.Fail($"An error occurred while disabling two-factor authentication: {ex.Message}");
            }
        }

        public async Task<IResponse<List<SessionInfo>>> GetSessionsAsync()
        {
            try
            {
                var result = await _httpService.GetAsync<List<DeviceSessionResponse>>(SecurityRoute.Devices);
                if (result.Succeeded && result.Data is not null)
                {
                    var sessions = result.Data.Select<DeviceSessionResponse, SessionInfo>(s => new SessionInfo
                    {
                        Id = s.Id.ToString(),
                        DeviceType = ParseDeviceType(s.UserAgent),
                        Browser = ParseBrowser(s.UserAgent),
                        Os = ParseOs(s.UserAgent),
                        Location = s.Location ?? "Unknown",
                        IpAddress = s.IpAddress,
                        LastActive = s.LastUsedAt ?? s.CreatedAt,
                        IsCurrent = s.IsCurrent
                    }).ToList();

                    return Response<List<SessionInfo>>.Success(sessions, "Sessions retrieved");
                }
                else
                {
                    return Response<List<SessionInfo>>.Fail(result.Messages ?? "Failed to load sessions");
                }
            }
            catch (Exception ex)
            {
                return Response<List<SessionInfo>>.Fail($"An error occurred while loading the sessions: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> RevokeSessionAsync(string id)
        {
            try
            {
                if (!int.TryParse(id, out var sessionId))
                {
                    return Response<bool>.Fail("Invalid session ID");
                }

                var result = await _httpService.DeleteAsync<ActionResponse>($"{SecurityRoute.Devices}/{sessionId}");
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Session revoked");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to revoke session");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while revoking the session: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> RevokeAllSessionsAsync(bool includingCurrent)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<ActionResponse>(
                    SecurityRoute.DevicesRevokeAll, new { includingCurrent });
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Sessions revoked");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to revoke sessions");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while revoking the sessions: {ex.Message}");
            }
        }

        public async Task<IResponse<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query)
        {
            try
            {
                var queryParams = new Dictionary<string, string?>
                {
                    ["from"] = query.From?.ToString("o"),
                    ["to"] = query.To?.ToString("o"),
                    ["type"] = query.Type,
                    ["search"] = query.Search
                };

                var queryString = string.Join("&", queryParams
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));

                var result = await _httpService.GetAsync<ActivityLogResponse>($"{SecurityRoute.Activity}?{queryString}");
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<List<ActivityLogEntry>>.Success(result.Data.Entries, "Activity retrieved");
                }
                else
                {
                    return Response<List<ActivityLogEntry>>.Fail(result.Messages ?? "Failed to load activity");
                }
            }
            catch (Exception ex)
            {
                return Response<List<ActivityLogEntry>>.Fail($"An error occurred while loading the activity: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> VerifyDeviceAsync(VerifyDeviceRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<ActionResponse>(SecurityRoute.VerifyDevice, request);
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Device verified");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to verify device");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred while verifying the device: {ex.Message}");
            }
        }

        private static string ParseDeviceType(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "desktop";
            var ua = userAgent.ToLowerInvariant();
            if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone")) return "mobile";
            if (ua.Contains("tablet") || ua.Contains("ipad")) return "tablet";
            return "desktop";
        }

        private static string ParseBrowser(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Firefox")) return "Firefox";
            if (userAgent.Contains("Safari")) return "Safari";
            if (userAgent.Contains("Edge")) return "Edge";
            return "Unknown";
        }

        private static string ParseOs(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";
            var ua = userAgent.ToLowerInvariant();
            if (ua.Contains("windows")) return "Windows";
            if (ua.Contains("mac os") || ua.Contains("macos")) return "macOS";
            if (ua.Contains("linux")) return "Linux";
            if (ua.Contains("android")) return "Android";
            if (ua.Contains("ios") || ua.Contains("iphone") || ua.Contains("ipad")) return "iOS";
            return "Unknown";
        }
    }
}
