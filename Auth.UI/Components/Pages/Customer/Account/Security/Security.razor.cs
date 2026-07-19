using global::Shared.UI.Components.Toaster;
using global::Shared.Core.UIModels.Security;
using Microsoft.AspNetCore.Components;
using global::Shared.UI.Manager.Interface.Auth;

namespace Auth.UI.Components.Pages.Customer.Account.Security
{
    public partial class Security : ComponentBase
    {
        [Inject] private ISecurityManager SecurityManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected SecuritySettings Settings { get; set; } = new();
        protected ChangePasswordRequest Pw { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected (int Score, string Label, string Color) Strength => PasswordStrength(Pw.NewPassword);

        protected override async Task OnInitializedAsync()
        {
            var result = await SecurityManager.GetSecurityAsync();
            if (result.Succeeded && result.Data is not null) Settings = result.Data;
        }

        protected async Task ChangePasswordAsync()
        {
            var result = await SecurityManager.ChangePasswordAsync(Pw);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded)
            {
                Pw = new ChangePasswordRequest();
                Toaster.ShowSuccess("Password changed");
            }
            else
            {
                Toaster.ShowDanger(StatusMessage);
            }
        }

        protected async Task ToggleTwoFactor(bool enabled)
        {
            var result = enabled
                ? await SecurityManager.EnableTwoFactorAsync()
                : await SecurityManager.DisableTwoFactorAsync();

            if (result.Succeeded && result.Data is not null)
            {
                Settings = result.Data;
                Toaster.ShowSuccess(enabled ? "Two-factor enabled" : "Two-factor disabled");
            }
            else
            {
                Toaster.ShowDanger(result.Messages ?? "Could not update 2FA");
            }
        }

        protected async Task SaveTwoFactorAsync()
        {
            var result = await SecurityManager.UpdateSecurityAsync(Settings);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess("2FA settings saved");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected async Task DisableTwoFactorAsync()
        {
            var result = await SecurityManager.DisableTwoFactorAsync();
            if (result.Succeeded && result.Data is not null)
            {
                Settings = result.Data;
                Toaster.ShowSuccess("Two-factor disabled");
            }
            else
            {
                Toaster.ShowDanger(result.Messages ?? "Could not disable 2FA");
            }
        }

        protected async Task SaveAlertsAsync()
        {
            var result = await SecurityManager.UpdateSecurityAsync(Settings);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess("Security alerts saved");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected (int Score, string Label, string Color) PasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (0, "Empty", "text-muted");

            int score = 0;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Any(char.IsUpper) && password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score++;
            score = Math.Min(score, 4);

            var label = score switch
            {
                0 => "Very weak",
                1 => "Weak",
                2 => "Fair",
                3 => "Good",
                _ => "Strong"
            };
            var color = score switch
            {
                0 => "text-danger",
                1 => "text-danger",
                2 => "text-warning",
                3 => "text-info",
                _ => "text-success"
            };
            return (score, label, color);
        }
    }
}
