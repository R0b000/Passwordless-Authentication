namespace Shared.UI.Manager.Routes
{
    public static class SecurityRoute
    {
        public const string Settings = "/api/security/settings";
        public const string ChangePassword = "/api/security/change-password";
        public const string Enable2FA = "/api/security/2fa/enable";
        public const string Disable2FA = "/api/security/2fa/disable";
        public const string Activity = "/api/security/activity";
        public const string VerifyDevice = "/api/security/device/verify";
        public const string Devices = "/api/auth/devices";
        public const string DevicesRevokeAll = "/api/auth/devices/logout-all";
    }
}
