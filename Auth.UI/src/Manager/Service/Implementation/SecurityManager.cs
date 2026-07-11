using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Security;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class SecurityManager : ISecurityManager
    {
        private readonly GenericHttpRepository<SecuritySettings> _settingsRepository;
        private readonly GenericHttpRepository<List<DeviceSessionResponse>> _sessionsRepository;
        private readonly GenericHttpRepository<ActivityLogResponse> _activityRepository;
        private readonly GenericHttpRepository<ActionResponse> _actionRepository;
        private readonly ITokenStore _tokenStore;

        public SecurityManager(
            GenericHttpRepository<SecuritySettings> settingsRepository,
            GenericHttpRepository<List<DeviceSessionResponse>> sessionsRepository,
            GenericHttpRepository<ActivityLogResponse> activityRepository,
            GenericHttpRepository<ActionResponse> actionRepository,
            ITokenStore tokenStore)
        {
            _settingsRepository = settingsRepository;
            _sessionsRepository = sessionsRepository;
            _activityRepository = activityRepository;
            _actionRepository = actionRepository;
            _tokenStore = tokenStore;
        }

        public async Task<Response<SecuritySettings>> GetSecurityAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<SecuritySettings>.Failure("No authentication token present");
            }

            return await _settingsRepository.GetSingleAsync("api/security/settings", token);
        }

        public async Task<Response<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<SecuritySettings>.Failure("No authentication token present");
            }

            return await _settingsRepository.QuerySingleAsync("api/security/settings", settings, token);
        }

        public async Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<bool>.Failure("No authentication token present");
            }

            var result = await _actionRepository.QuerySingleAsync("api/security/change-password", request, token);
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Password changed");
        }

        public async Task<Response<SecuritySettings>> EnableTwoFactorAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<SecuritySettings>.Failure("No authentication token present");
            }

            return await _settingsRepository.QuerySingleAsync("api/security/2fa/enable", null, token);
        }

        public async Task<Response<SecuritySettings>> DisableTwoFactorAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<SecuritySettings>.Failure("No authentication token present");
            }

            return await _settingsRepository.QuerySingleAsync("api/security/2fa/disable", null, token);
        }

        public async Task<Response<List<SessionInfo>>> GetSessionsAsync()
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<List<SessionInfo>>.Failure("No authentication token present");
            }

            var result = await _sessionsRepository.GetSingleAsync("api/auth/devices", token);
            if (result == null || result.Data == null)
            {
                return Response<List<SessionInfo>>.Failure("Failed to load sessions");
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
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<bool>.Failure("No authentication token present");
            }

            if (!int.TryParse(id, out var sessionId))
            {
                return Response<bool>.Failure("Invalid session ID");
            }

            var result = await _actionRepository.QuerySingleAsync($"api/auth/devices/{sessionId}", null, token);
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Session revoked");
        }

        public async Task<Response<bool>> RevokeAllSessionsAsync(bool includingCurrent)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<bool>.Failure("No authentication token present");
            }

            var result = await _actionRepository.QuerySingleAsync("api/auth/devices/logout-all", new { includingCurrent }, token);
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Sessions revoked");
        }

        public async Task<Response<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<List<ActivityLogEntry>>.Failure("No authentication token present");
            }

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

            var result = await _activityRepository.GetSingleAsync($"api/security/activity?{queryString}", token);
            if (result == null)
            {
                return Response<List<ActivityLogEntry>>.Failure("Failed to load activity");
            }

            return Response<List<ActivityLogEntry>>.Success(result.Data.Entries);
        }

        public async Task<Response<bool>> VerifyDeviceAsync(VerifyDeviceRequest request)
        {
            var token = _tokenStore.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Response<bool>.Failure("No authentication token present");
            }

            var result = await _actionRepository.QuerySingleAsync("api/security/device/verify", request, token);
            return Response<bool>.Success(result?.Succeeded ?? false, result?.Message ?? "Device verified");
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
