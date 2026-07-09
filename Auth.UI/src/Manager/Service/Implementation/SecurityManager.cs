using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Security;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class SecurityManager : ISecurityManager
    {
        private static readonly Random _rng = new();

        private static SecuritySettings _settings = new()
        {
            LastPasswordChange = DateTime.Now.AddDays(-46),
            TwoFactorEnabled = false,
            TwoFactorMethod = "authenticator",
            AlertOnNewDevice = true,
            RequirePasswordForSensitive = true
        };

        private static readonly List<SessionInfo> _sessions = new()
        {
            new SessionInfo { Id = "s1", DeviceType = "desktop", Browser = "Chrome 124", Os = "macOS 14", Location = "New York, US", IpAddress = "203.0.113.42", LastActive = DateTime.Now.AddMinutes(-2), IsCurrent = true },
            new SessionInfo { Id = "s2", DeviceType = "mobile", Browser = "Safari 17", Os = "iOS 17", Location = "New York, US", IpAddress = "203.0.113.77", LastActive = DateTime.Now.AddHours(-5) },
            new SessionInfo { Id = "s3", DeviceType = "tablet", Browser = "Edge 124", Os = "Windows 11", Location = "London, UK", IpAddress = "198.51.100.9", LastActive = DateTime.Now.AddDays(-1) }
        };

        private static readonly List<ActivityLogEntry> _activity = new()
        {
            new ActivityLogEntry { Id = 1, Type = "login", Description = "Successful login", Device = "Chrome 124 · macOS", IpAddress = "203.0.113.42", Timestamp = DateTime.Now.AddMinutes(-2) },
            new ActivityLogEntry { Id = 2, Type = "settings_update", Description = "Updated notification preferences", Device = "Chrome 124 · macOS", IpAddress = "203.0.113.42", Timestamp = DateTime.Now.AddHours(-1) },
            new ActivityLogEntry { Id = 3, Type = "password_change", Description = "Password changed", Device = "Chrome 124 · macOS", IpAddress = "203.0.113.42", Timestamp = DateTime.Now.AddDays(-2) },
            new ActivityLogEntry { Id = 4, Type = "login", Description = "Successful login", Device = "Safari 17 · iOS", IpAddress = "203.0.113.77", Timestamp = DateTime.Now.AddHours(-5) },
            new ActivityLogEntry { Id = 5, Type = "logout", Description = "Signed out", Device = "Edge 124 · Windows", IpAddress = "198.51.100.9", Timestamp = DateTime.Now.AddDays(-1) },
            new ActivityLogEntry { Id = 6, Type = "2fa", Description = "Two-factor authentication enabled", Device = "Chrome 124 · macOS", IpAddress = "203.0.113.42", Timestamp = DateTime.Now.AddDays(-3) },
            new ActivityLogEntry { Id = 7, Type = "login_failed", Description = "Failed login attempt", Device = "Unknown · Linux", IpAddress = "192.0.2.15", Timestamp = DateTime.Now.AddDays(-4) },
            new ActivityLogEntry { Id = 8, Type = "settings_update", Description = "Updated privacy preferences", Device = "Chrome 124 · macOS", IpAddress = "203.0.113.42", Timestamp = DateTime.Now.AddDays(-6) }
        };

        public Task<Response<SecuritySettings>> GetSecurityAsync()
        {
            EnsureTwoFactorArtifacts();
            return Task.FromResult(Response<SecuritySettings>.Success(Clone(_settings)));
        }

        public Task<Response<SecuritySettings>> UpdateSecurityAsync(SecuritySettings settings)
        {
            _settings = settings;
            return Task.FromResult(Response<SecuritySettings>.Success(Clone(_settings), "Security settings saved"));
        }

        public Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return Task.FromResult(Response<bool>.Failure("Please complete all password fields"));
            if (request.NewPassword != request.ConfirmPassword)
                return Task.FromResult(Response<bool>.Failure("New password and confirmation do not match"));

            _settings.LastPasswordChange = DateTime.Now;
            return Task.FromResult(Response<bool>.Success(true, "Password changed successfully"));
        }

        public Task<Response<SecuritySettings>> EnableTwoFactorAsync()
        {
            _settings.TwoFactorEnabled = true;
            EnsureTwoFactorArtifacts();
            return Task.FromResult(Response<SecuritySettings>.Success(Clone(_settings), "Two-factor authentication enabled"));
        }

        public Task<Response<SecuritySettings>> DisableTwoFactorAsync()
        {
            _settings.TwoFactorEnabled = false;
            _settings.BackupCodes = new List<string>();
            _settings.QrCodeUri = null;
            return Task.FromResult(Response<SecuritySettings>.Success(Clone(_settings), "Two-factor authentication disabled"));
        }

        public Task<Response<List<SessionInfo>>> GetSessionsAsync()
            => Task.FromResult(Response<List<SessionInfo>>.Success(_sessions.Select(Clone).ToList()));

        public Task<Response<bool>> RevokeSessionAsync(string id)
        {
            var removed = _sessions.RemoveAll(s => s.Id == id) > 0;
            return Task.FromResult(removed
                ? Response<bool>.Success(true, "Session revoked")
                : Response<bool>.Failure("Session not found"));
        }

        public Task<Response<bool>> RevokeAllSessionsAsync(bool includingCurrent)
        {
            if (includingCurrent)
                _sessions.Clear();
            else
                _sessions.RemoveAll(s => !s.IsCurrent);

            return Task.FromResult(Response<bool>.Success(true, includingCurrent ? "Signed out of all devices" : "Signed out of all other devices"));
        }

        public Task<Response<List<ActivityLogEntry>>> GetActivityAsync(ActivityQuery query)
        {
            var result = _activity.AsEnumerable();

            if (query.From.HasValue)
                result = result.Where(a => a.Timestamp >= query.From.Value);
            if (query.To.HasValue)
                result = result.Where(a => a.Timestamp <= query.To.Value.Date.AddDays(1).AddTicks(-1));
            if (!string.IsNullOrWhiteSpace(query.Type) && query.Type != "all")
                result = result.Where(a => a.Type == query.Type);
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim().ToLowerInvariant();
                result = result.Where(a =>
                    a.Description.ToLowerInvariant().Contains(term) ||
                    a.Device.ToLowerInvariant().Contains(term) ||
                    a.IpAddress.ToLowerInvariant().Contains(term));
            }

            return Task.FromResult(Response<List<ActivityLogEntry>>.Success(result.OrderByDescending(a => a.Timestamp).ToList()));
        }

        public Task<Response<bool>> VerifyDeviceAsync(VerifyDeviceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return Task.FromResult(Response<bool>.Failure("Enter the verification code"));

            return Task.FromResult(Response<bool>.Success(true, request.TrustDevice
                ? "Device verified and trusted"
                : "Device verified"));
        }

        private void EnsureTwoFactorArtifacts()
        {
            if (_settings.TwoFactorEnabled && string.IsNullOrEmpty(_settings.QrCodeUri))
            {
                _settings.QrCodeUri = "otpauth://totp/PasswordlessAuth:jane.doe@example.com?secret=JBSWY3DPEHPK3PXP&issuer=PasswordlessAuth";
            }

            if (_settings.TwoFactorEnabled && (_settings.BackupCodes is null || _settings.BackupCodes.Count == 0))
            {
                _settings.BackupCodes = Enumerable.Range(0, 10).Select(_ => RandomCode()).ToList();
            }
        }

        private static string RandomCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var s = new System.Text.StringBuilder();
            for (int i = 0; i < 4; i++) s.Append(chars[_rng.Next(chars.Length)]);
            s.Append('-');
            for (int i = 0; i < 4; i++) s.Append(chars[_rng.Next(chars.Length)]);
            return s.ToString();
        }

        private static T Clone<T>(T item) where T : class
            => System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(item))!;
    }
}
