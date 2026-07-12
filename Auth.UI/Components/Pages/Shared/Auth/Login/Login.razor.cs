using Auth.UI.Components.UI.Modal;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Auth.UI.Components.Pages.Shared.Login
{
    public partial class Login : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected string Mode { get; set; } = "login";
        protected RegisterRequest RegisterModel { get; set; } = new();
        protected LoginRequest LoginModel { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected int LoggedInUserId { get; set; }
        protected bool ShowPassword { get; set; }

        // NEW: Tracks whether we are verifying an existing passkey or registering a new one
        protected bool IsVerificationMode { get; set; }
        protected string LoggedInUsername { get; set; } = string.Empty;

        protected void TogglePassword() => ShowPassword = !ShowPassword;

        protected void SocialAsync(string provider)
        {
            var message = provider switch
            {
                "Google" => "Google sign-in is not configured in this demo.",
                "Microsoft" => "Microsoft sign-in is not configured in this demo.",
                "reset" => "Password recovery is not configured in this demo.",
                _ => "This sign-in method is not configured in this demo."
            };
            Toaster.ShowInfo(message);
        }

        protected async Task RegisterAsync()
        {
            StatusMessage = string.Empty;

            var result = await AuthController.RegisterAsync(RegisterModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Registration failed";

            if (result.Succeeded && result.Data is not null)
            {
                Mode = "login";
                LoginModel.Username = RegisterModel.Username;
                StatusMessage = "Registration successful. You can now sign in with your credentials and add a passkey from your profile.";
            }
        }

        protected async Task LoginAsync()
        {
            StatusMessage = string.Empty;

            var result = await AuthController.LoginAsync(LoginModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Login failed";

            if (result.Succeeded)
            {
                if (result.Data?.RequiresFido2 == true)
                {
                    // EXISTING: User has passkey, needs to VERIFY
                    LoggedInUserId = result.Data.UserId;
                    IsVerificationMode = true;
                    PasskeyVisible = true;
                }
                else if (result.Data?.RequiresFido2Registration == true)
                {
                    // NEW: User doesn't have passkey, needs to REGISTER
                    LoggedInUserId = result.Data.UserId;
                    LoggedInUsername = LoginModel.Username;
                    IsVerificationMode = false;
                    PasskeyVisible = true;
                }
                else
                {
                    // Standard login without FIDO2
                    NavigationManager.NavigateTo("/profile");
                }
            }
        }

        protected Modal PasskeyModal { get; set; } = default!;
        protected bool PasskeyVisible { get; set; }

        // UPDATED: Dynamically compute the title based on the mode
        protected string PasskeyModalTitle => LoggedInUserId > 0
            ? (IsVerificationMode ? "Verify it's you" : "Set up a passkey")
            : "Sign in with a passkey";

        protected void OpenPasskeyModal()
        {
            PasskeyVisible = true;
        }

        protected void HandlePasskeyCompleted()
        {
            PasskeyVisible = false;
            StatusMessage = string.Empty;
            // Redirect to profile after successful setup or verification
            NavigationManager.NavigateTo("/profile");
        }

        // NEW/UPDATED: Handle the "Skip for now" button from PasskeySetup
        protected void HandlePasskeySkipped()
        {
            PasskeyVisible = false;
            StatusMessage = string.Empty; // Clear any errors
                                          // User skipped passkey setup, but they are already logged in via password.
                                          // Gracefully redirect without showing any errors.
            NavigationManager.NavigateTo("/profile");
        }

        // UPDATED: Handle the Modal "Close" (X) button
        protected void OnPasskeyCancel()
        {
            PasskeyVisible = false;
            LoggedInUserId = 0;
            IsVerificationMode = false;
            StatusMessage = string.Empty; // Clear any errors

            // If they close the modal, they are still logged in via password. 
            // Let them into the app without errors.
            NavigationManager.NavigateTo("/profile");
        }
    }
}