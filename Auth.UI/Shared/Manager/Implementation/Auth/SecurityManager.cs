using Auth.UI.Shared.Common;
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
            return await _httpService.GetAsync<SecuritySettings>(SecurityRoute.Settings);
        }

        public async Task<IResponse<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings)
        {
            return await _httpService.PostAsJsonAsync<SecuritySettings>(SecurityRoute.Settings, settings);
        }

        public async Task<IResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var result = await _httpService.PostAsJsonAsync<ActionResponse>(SecurityRoute.ChangePassword, request);
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Password changed");
        }

        public async Task<IResponse<SecuritySettings>> EnableTwoFactorAsync()
        {
            return await _httpService.PostAsJsonAsync<SecuritySettings>(SecurityRoute.Enable2FA, null!);
        }

        public async Task<IResponse<SecuritySettings>> DisableTwoFactorAsync()
        {
            return await _httpService.PostAsJsonAsync<SecuritySettings>(SecurityRoute.Disable2FA, null!);
        }

        public async Task<IResponse<List<SessionInfo>>> GetSessionsAsync()
        {
            var result = await _httpService.GetAsync<List<DeviceSessionResponse>>(SecurityRoute.Devices);
            if (!result.Succeeded || result.Data is null)
            {
                return Response<List<SessionInfo>>.Failure(result.Message ?? "Failed to load sessions");
            }

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

            return Response<List<SessionInfo>>.Success(sessions);
        }

        public async Task<IResponse<bool>> RevokeSessionAsync(string id)
        {
            if (!int.TryParse(id, out var sessionId))
            {
                return Response<bool>.Failure("Invalid session ID");
            }

            var result = await _httpService.DeleteAsync<ActionResponse>($"{SecurityRoute.Devices}/{sessionId}");
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Session revoked");
        }

        public async Task<IResponse<bool>> RevokeAllSessionsAsync(bool includingCurrent)
        {
            var result = await _httpService.PostAsJsonAsync<ActionResponse>(
                SecurityRoute.DevicesRevokeAll, new { includingCurrent });
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Sessions revoked");
        }

        public async Task<IResponse<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query)
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
            if (!result.Succeeded || result.Data is null)
            {
                return Response<List<ActivityLogEntry>>.Failure(result.Message ?? "Failed to load activity");
            }

            return Response<List<ActivityLogEntry>>.Success(result.Data.Entries);
        }

        public async Task<IResponse<bool>> VerifyDeviceAsync(VerifyDeviceRequest request)
        {
            var result = await _httpService.PostAsJsonAsync<ActionResponse>(SecurityRoute.VerifyDevice, request);
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Device verified");
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
