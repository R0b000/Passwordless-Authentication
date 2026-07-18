using System.ComponentModel.DataAnnotations;

namespace Shared.Core.Models.RequestModel.Account
{
    public class UpdateProfileRequest
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }

    public class UpdateSettingsRequest
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailPreferences { get; set; }
        public string Timezone { get; set; } = "UTC";
        public string Language { get; set; } = "en";
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool SmsAlerts { get; set; }
        public bool MarketingEmails { get; set; }
    }

    public class UpdatePrivacyRequest
    {
        public string ProfileVisibility { get; set; } = "private";
        public bool DataSharing { get; set; }
        public bool ThirdPartyConnections { get; set; }
        public string CookiePreferences { get; set; } = "essential";
    }

    public class PasswordResetRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ConfirmPasswordResetRequest
    {
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters and contain uppercase, lowercase, and a number")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters and contain uppercase, lowercase, and a number")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

