using Microsoft.AspNetCore.Components;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Auth;

namespace Auth.UI.Components.Pages.Shared.Auth.Reset
{
    public partial class ResetPasswordPage 
    {

        [SupplyParameterFromQuery]
        public string Email { get; set; } = string.Empty;

        protected ResetPasswordRequest RequestModel { get; set; } = new();
        protected bool ShowPassword { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected bool IsSubmitting { get; set; }

        protected string PasswordStrengthText => ComputePasswordStrength();
        protected int PasswordStrengthPercent => ComputePasswordStrengthPercent();

        protected string ComputePasswordStrength()
        {
            var pw = RequestModel.NewPassword ?? string.Empty;
            if (pw.Length == 0) return string.Empty;
            if (pw.Length < 6) return "Very weak";
            if (pw.Length < 8) return "Weak";
            var score = 0;
            if (pw.Any(char.IsLower)) score++;
            if (pw.Any(char.IsUpper)) score++;
            if (pw.Any(char.IsDigit)) score++;
            if (pw.Any(c => !char.IsLetterOrDigit(c))) score++;
            return score switch
            {
                <= 1 => "Weak",
                2 => "Fair",
                3 => "Good",
                _ => "Strong"
            };
        }

        protected int ComputePasswordStrengthPercent()
        {
            var pw = RequestModel.NewPassword ?? string.Empty;
            if (pw.Length == 0) return 0;
            var score = 0;
            if (pw.Length >= 8) score += 25;
            if (pw.Length >= 12) score += 15;
            if (pw.Any(char.IsLower)) score += 15;
            if (pw.Any(char.IsUpper)) score += 15;
            if (pw.Any(char.IsDigit)) score += 15;
            if (pw.Any(c => !char.IsLetterOrDigit(c))) score += 15;
            return Math.Min(score, 100);
        }

        protected void TogglePassword()
        {
            ShowPassword = !ShowPassword;
        }

        protected override void OnInitialized()
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                RequestModel.Email = Email.Trim();
            }
        }

        protected async Task SubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Succeeded = false;
                StatusMessage = "Email is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(RequestModel.Otp))
            {
                Succeeded = false;
                StatusMessage = "Please enter the verification code.";
                return;
            }

            if (string.IsNullOrWhiteSpace(RequestModel.NewPassword))
            {
                Succeeded = false;
                StatusMessage = "Please enter a new password.";
                return;
            }

            if (RequestModel.NewPassword != RequestModel.ConfirmPassword)
            {
                Succeeded = false;
                StatusMessage = "Passwords do not match.";
                return;
            }

            if (RequestModel.NewPassword.Length < 8)
            {
                Succeeded = false;
                StatusMessage = "Password must be at least 8 characters.";
                return;
            }

            try
            {
                IsSubmitting = true;
                Loader.Show("Resetting password...");
                var result = await AccountManager.ResetPasswordAsync(new ResetPasswordRequest
                {
                    Email = Email.Trim(),
                    Otp = RequestModel.Otp.Trim(),
                    NewPassword = RequestModel.NewPassword,
                    ConfirmPassword = RequestModel.ConfirmPassword
                });

                Succeeded = result.Succeeded;
                StatusMessage = result.Messages ?? string.Empty;

                if (result.Succeeded)
                {
                    Toaster.ShowSuccess("Password reset successful. Please sign in.");
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    Toaster.ShowDanger(StatusMessage);
                }
            }
            finally
            {
                IsSubmitting = false;
                Loader.Hide();
            }
        }
    }
}