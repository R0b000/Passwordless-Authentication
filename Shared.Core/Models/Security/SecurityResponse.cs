namespace Shared.Core.Models.Security
{
    public class SecuritySettingsResponse
    {
        public DateTime? LastPasswordChange { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorMethod { get; set; } = "authenticator";
        public string? QrCodeUri { get; set; }
        public List<string> BackupCodes { get; set; } = new();
        public bool AlertOnNewDevice { get; set; } = true;
        public bool RequirePasswordForSensitive { get; set; } = true;
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
    /// <summary>
    /// Gets the stored authentication token.
    /// </summary>
    /// <returns>The stored token or null if no token is set.</returns>
        public bool IsCurrent { get; set; }
    }

    public class ActiveSessionsResponse
    {
        public List<DeviceSessionResponse> Sessions { get; set; } = new();
        public int MaxAllowedSessions { get; set; } = 5;
    }

    public class ActivityLogResponse
    {
        public List<ActivityLogEntry> Entries { get; set; } = new();
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
}
