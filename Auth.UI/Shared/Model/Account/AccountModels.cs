namespace Auth.UI.Shared.Model.Account
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
        public string AccountStatus { get; set; } = "active";
    }

    public class AccountSettings
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailPreferences { get; set; }
        public string Timezone { get; set; } = "UTC";
        public string Language { get; set; } = "en";
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; }
        public bool SmsAlerts { get; set; }
        public bool MarketingEmails { get; set; }
    }

    public class PrivacySettings
    {
        public string ProfileVisibility { get; set; } = "private";
        public bool DataSharing { get; set; }
        public bool ThirdPartyConnections { get; set; }
        public string CookiePreferences { get; set; } = "essential";
    }
}
