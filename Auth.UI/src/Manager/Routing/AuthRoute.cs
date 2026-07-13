namespace Auth.UI.src.Manager.Routing
{
    /// <summary>
    /// Central, static definitions for all authentication-related route URIs.
    /// Keeps navigation targets in one place and avoids magic strings across the UI.
    /// </summary>
    public static class AuthRoute
    {
        public const string Home = "/";
        public const string Login = "/login";
        public const string Register = "/signup";
        public const string ForgotPassword = "/forgot-password";
        public const string ResetPassword = "/reset-password";
        public const string VerifyDevice = "/verify-device";
        public const string Passkey = "/fido2";
        public const string Profile = "/profile";

        public const string AccountProfile = "/account/profile";
        public const string AccountSettings = "/account/settings";
        public const string AccountSecurity = "/account/security";
        public const string AccountSessions = "/account/sessions";
        public const string AccountActivity = "/account/activity";
        public const string AccountPrivacy = "/account/privacy";

        public const string Showcase = "/showcase";
    }
}
