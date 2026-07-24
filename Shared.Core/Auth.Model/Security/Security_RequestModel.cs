namespace Auth.Model.Models.Security
{
    public class SecuritySettingsRequest
    {
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorMethod { get; set; } = "authenticator";
        public bool AlertOnNewDevice { get; set; } = true;
        public bool RequirePasswordForSensitive { get; set; } = true;
    }

    public class ActivityQueryRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Type { get; set; }
        public string? Search { get; set; }
    }
    /// <summary>
    /// Sets the authentication token.
    /// </summary>
    /// <param name="token">The token to be stored.</param>

    public class VerifyDeviceRequest
    {
        public string Code { get; set; } = string.Empty;
        public bool TrustDevice { get; set; }
    }
}

