namespace Shared.Core.Models.RequestModel.Security
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

    public class VerifyDeviceRequest
    {
        public string Code { get; set; } = string.Empty;
        public bool TrustDevice { get; set; }
    }
}
