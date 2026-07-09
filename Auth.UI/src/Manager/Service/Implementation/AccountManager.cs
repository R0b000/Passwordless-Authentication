using Auth.UI.src.Common;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Account;
using Auth.UI.src.Model.Auth;

namespace Auth.UI.src.Manager.Service.Implementation
{
    public class AccountManager : IAccountManager
    {
        private static UserProfile _profile = new()
        {
            UserId = 1,
            Username = "jdoe",
            DisplayName = "Jane Doe",
            Email = "jane.doe@example.com",
            Phone = "+1 555 010 2233",
            Bio = "Product designer who loves hiking and quiet cafés.",
            AvatarUrl = string.Empty,
            DateJoined = new DateTime(2023, 4, 12),
            LastActive = DateTime.Now.AddMinutes(-8),
            LastLoginAt = DateTime.Now.AddHours(-3),
            LastLoginIp = "203.0.113.42",
            AccountStatus = "active"
        };

        private static AccountSettings _settings = new()
        {
            DisplayName = "Jane Doe",
            Username = "jdoe",
            Email = "jane.doe@example.com",
            EmailPreferences = true,
            Timezone = "America/New_York",
            Language = "en",
            EmailNotifications = true,
            PushNotifications = false,
            SmsAlerts = true,
            MarketingEmails = false
        };

        private static PrivacySettings _privacy = new()
        {
            ProfileVisibility = "private",
            DataSharing = false,
            ThirdPartyConnections = true,
            CookiePreferences = "essential"
        };

        public Task<Response<UserProfile>> GetProfileAsync()
            => Task.FromResult(Response<UserProfile>.Success(Clone(_profile)));

        public Task<Response<UserProfile>> UpdateProfileAsync(UserProfile profile)
        {
            _profile = profile;
            return Task.FromResult(Response<UserProfile>.Success(Clone(_profile), "Profile updated"));
        }

        public Task<Response<AccountSettings>> GetSettingsAsync()
            => Task.FromResult(Response<AccountSettings>.Success(Clone(_settings)));

        public Task<Response<AccountSettings>> UpdateSettingsAsync(AccountSettings settings)
        {
            _settings = settings;
            return Task.FromResult(Response<AccountSettings>.Success(Clone(_settings), "Settings saved"));
        }

        public Task<Response<PrivacySettings>> GetPrivacyAsync()
            => Task.FromResult(Response<PrivacySettings>.Success(Clone(_privacy)));

        public Task<Response<PrivacySettings>> UpdatePrivacyAsync(PrivacySettings privacy)
        {
            _privacy = privacy;
            return Task.FromResult(Response<PrivacySettings>.Success(Clone(_privacy), "Privacy preferences updated"));
        }

        public Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
            => Task.FromResult(Response<AuthResponse>.Success(new AuthResponse
            {
                UserId = 1,
                Username = request.Username,
                Email = request.Email,
                Message = "Registration successful (demo)"
            }, "Account created"));

        public Task<Response<bool>> RequestPasswordResetAsync(string email)
            => Task.FromResult(Response<bool>.Success(true, $"If {email} exists, a reset link was sent (demo)"));

        public Task<Response<bool>> ResetPasswordAsync(string token, string newPassword)
            => Task.FromResult(Response<bool>.Success(true, "Password has been reset (demo)"));

        public Task<Response<string>> DownloadDataAsync()
            => Task.FromResult(Response<string>.Success("https://example.com/data-export/demo.zip", "Export prepared (demo)"));

        public Task<Response<bool>> DeleteAccountAsync()
            => Task.FromResult(Response<bool>.Success(true, "Account scheduled for deletion (demo)"));

        private static T Clone<T>(T item) where T : class
            => System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(item))!;
    }
}
