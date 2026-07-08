using Auth.UI.Components.Pages;
using Auth.UI.Components.UI.Modal;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Auth.UI.Components.Pages
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
                    LoggedInUserId = result.Data.UserId;
                    PasskeyVisible = true;
                }
                else
                {
                    NavigationManager.NavigateTo("/profile");
                }
            }
        }

        protected Modal PasskeyModal { get; set; } = default!;
        protected bool PasskeyVisible { get; set; }

        protected string PasskeyModalTitle => LoggedInUserId > 0 ? "Verify it's you" : "Sign in with a passkey";

        protected void OpenPasskeyModal()
        {
            PasskeyVisible = true;
        }

        protected void HandlePasskeyCompleted()
        {
            PasskeyVisible = false;
        }

        protected void OnPasskeyCancel()
        {
            PasskeyVisible = false;
            LoggedInUserId = 0;
        }
    }
}
