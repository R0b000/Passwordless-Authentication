namespace Auth.UI.src.Model.Security
{
    public class SecuritySettings
    {
        public DateTime? LastPasswordChange { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorMethod { get; set; } = "authenticator";
        public string? QrCodeUri { get; set; }
        public List<string> BackupCodes { get; set; } = new();
        public bool AlertOnNewDevice { get; set; } = true;
        public bool RequirePasswordForSensitive { get; set; } = true;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class SessionInfo
    {
        public string Id { get; set; } = string.Empty;
        public string DeviceType { get; set; } = "desktop";
        public string Browser { get; set; } = string.Empty;
        public string Os { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class ActivityQuery
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Type { get; set; }
        public string? Search { get; set; }
    }

    public class VerifyDeviceRequest
    {
        public string Code { get; set; } = string.Empty;
        public bool TrustDevice { get; set; }
    }

    public class DeviceSessionResponse
    {
        public int Id { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ActiveSessionsResponse
    {
        public List<SessionInfo> Sessions { get; set; } = new();
    }

    public class ActivityLogResponse
    {
        public List<ActivityLogEntry> Entries { get; set; } = new();
    }

    public class ActionResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
