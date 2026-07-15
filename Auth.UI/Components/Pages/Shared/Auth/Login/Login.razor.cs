using Auth.UI.Components.UI.Modal;
using Auth.UI.Shared.Components.Toaster;
using Auth.UI.Shared.Model.Auth;
using Auth.UI.Shared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using UI.Shared.Manager.Interface.Auth;

namespace Auth.UI.Components.Pages.Shared.Login
{
    public partial class Login : ComponentBase
    {
        [Inject] private IAuthManager AuthManager { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;
        [Inject] private ITokenStore TokenStore { get; set; } = default!;

        protected string Mode { get; set; } = "login";
        protected RegisterRequest RegisterModel { get; set; } = new();
        protected LoginRequest LoginModel { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected int LoggedInUserId { get; set; }
        protected bool ShowPassword { get; set; }

        protected bool IsVerificationMode { get; set; }
        protected string LoggedInUsername { get; set; } = string.Empty;
        protected bool _redirectToProfile;

        protected override void OnInitialized()
        {
            if (TokenStore.GetToken() is not null)
            {
                _redirectToProfile = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_redirectToProfile)
            {
                NavigationManager.NavigateTo("/profile", replace: true);
            }
        }

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

            var result = await AuthManager.RegisterAsync(RegisterModel);
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

            var result = await AuthManager.LoginAsync(LoginModel);
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

        protected void HandlePasskeySkipped()
        {
            PasskeyVisible = false;
            StatusMessage = string.Empty;
            LoggedInUserId = 0;
            IsVerificationMode = false;
            NavigationManager.NavigateTo("/profile");
        }

        protected void OnPasskeyCancel()
        {
            var wasVerification = IsVerificationMode;
            PasskeyVisible = false;
            LoggedInUserId = 0;
            IsVerificationMode = false;
            StatusMessage = string.Empty;

            if (wasVerification)
            {
                TokenStore.Clear();
            }
            else
            {
                NavigationManager.NavigateTo("/profile");
            }
        }
    }
}