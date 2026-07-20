namespace Shared.UI.Manager.Routes
{
    public static class AuthRoute
    {
        public const string Login = "/api/auth/login";
        public const string Register = "/api/auth/register";
        public const string Me = "/api/auth/me";
        public const string Lookup = "/api/auth/lookup";
        public const string Fido2Options = "/api/auth/fido2/options/register";
        public const string Fido2Register = "/api/auth/fido2/register";
        public const string Fido2Challenge = "/api/auth/fido2/challenge";
        public const string Fido2Verify = "/api/auth/fido2/verify";
        public const string OtpRequest = "/api/auth/otp/request";
        public const string OtpVerify = "/api/auth/otp/verify";
    }
}
