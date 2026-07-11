namespace PasswordlessApi.Api.Configuration
{
    public class SecuritySettings
    {
        public int MaxLoginAttempts { get; set; } = 5;
        public TimeSpan LoginLockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
        public int MaxConcurrentSessions { get; set; } = 5;
        public bool EnableSuspiciousActivityDetection { get; set; } = true;
        public bool ForceReauthOnLocationChange { get; set; } = true;
    }
}