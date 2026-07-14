using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Security;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class SecurityManager : ISecurityManager
    {
        private readonly IHttpService _httpService;
        private readonly ITokenStore _tokenStore;

        public SecurityManager(IHttpService httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
        }

        public async Task<Response<SecuritySettings>> GetSecurityAsync()
        {
            return await _httpService.GetAsync<SecuritySettings>(SecurityRoute.Settings);
        }

        public async Task<Response<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings)
        {
            return await _httpService.PostAsync<SecuritySettings, SecuritySettings>(SecurityRoute.Settings, settings);
        }

        public async Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var result = await _httpService.PostAsync<ChangePasswordRequest, ActionResponse>(SecurityRoute.ChangePassword, request);
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Password changed");
        }

        public async Task<Response<SecuritySettings>> EnableTwoFactorAsync()
        {
            return await _httpService.PostAsync<object, SecuritySettings>(SecurityRoute.Enable2FA, null!);
        }

        public async Task<Response<SecuritySettings>> DisableTwoFactorAsync()
        {
            return await _httpService.PostAsync<object, SecuritySettings>(SecurityRoute.Disable2FA, null!);
        }

        public async Task<Response<List<SessionInfo>>> GetSessionsAsync()
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

        public async Task<Response<bool>> RevokeSessionAsync(string id)
        {
            if (!int.TryParse(id, out var sessionId))
            {
                return Response<bool>.Failure("Invalid session ID");
            }

            var result = await _httpService.DeleteAsync<ActionResponse>($"{SecurityRoute.Devices}/{sessionId}");
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Session revoked");
        }

        public async Task<Response<bool>> RevokeAllSessionsAsync(bool includingCurrent)
        {
            var result = await _httpService.PostAsync<object, ActionResponse>(
                SecurityRoute.DevicesRevokeAll, new { includingCurrent });
            return Response<bool>.Success(result.Succeeded, result.Message ?? "Sessions revoked");
        }

        public async Task<Response<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query)
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

        public async Task<Response<bool>> VerifyDeviceAsync(VerifyDeviceRequest request)
        {
            var result = await _httpService.PostAsync<VerifyDeviceRequest, ActionResponse>(SecurityRoute.VerifyDevice, request);
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
