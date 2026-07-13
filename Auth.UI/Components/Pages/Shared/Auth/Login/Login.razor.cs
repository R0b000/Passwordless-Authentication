using Auth.UI.Components.UI.Modal;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Routing;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Auth;
using Auth.UI.src.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

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
                NavigationManager.NavigateTo(AuthRoute.Profile, replace: true);
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

            // The API returns HTTP 200 even for failed logins (e.g. bad password), so a
            // successful HTTP response does NOT mean the user is authenticated. Only treat the
            // login as successful when the response actually carries an auth token or a
            // required FIDO2 step. Otherwise show the error and stay on the login page.
            if (!result.Succeeded || result.Data is null)
            {
                Succeeded = false;
                StatusMessage = result.Message ?? "Login failed";
                return;
            }

            var data = result.Data;
            if (data.RequiresFido2)
            {
                // User has a passkey and must verify it.
                Succeeded = true;
                LoggedInUserId = data.UserId;
                IsVerificationMode = true;
                PasskeyVisible = true;
            }
            else if (data.RequiresFido2Registration)
            {
                // User exists but has no passkey yet and must register one.
                Succeeded = true;
                LoggedInUserId = data.UserId;
                LoggedInUsername = LoginModel.Username;
                IsVerificationMode = false;
                PasskeyVisible = true;
            }
            else if (!string.IsNullOrEmpty(data.Token))
            {
                // Standard login without FIDO2: a token means we are authenticated.
                Succeeded = true;
                NavigationManager.NavigateTo(AuthRoute.Profile);
            }
            else
            {
                // Login failed (invalid credentials). Do NOT navigate; surface the error.
                Succeeded = false;
                StatusMessage = data.Message ?? "Invalid username or password";
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
            // Standalone passkey sign-in: the FIDO2/Passkey ceremony must not start until
            // the user's identity is known. Entering verification mode presents the
            // PasskeyLogin component, which requires the email to be entered and validated
            // (account resolved) BEFORE the passkey process is initiated. Without this, the
            // flow would begin with UserId = 0 and registration/verification would fail.
            IsVerificationMode = true;
            LoggedInUserId = 0;
            LoggedInUsername = string.Empty;
            PasskeyVisible = true;
        }

        protected void HandlePasskeyCompleted()
        {
            PasskeyVisible = false;
            StatusMessage = string.Empty;
            // Redirect to profile after successful setup or verification
            NavigationManager.NavigateTo(AuthRoute.Profile);
        }

        protected void HandlePasskeySkipped()
        {
            PasskeyVisible = false;
            StatusMessage = string.Empty;
            LoggedInUserId = 0;
            IsVerificationMode = false;
            NavigationManager.NavigateTo(AuthRoute.Profile);
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
                NavigationManager.NavigateTo(AuthRoute.Profile);
            }
        }
    }
}