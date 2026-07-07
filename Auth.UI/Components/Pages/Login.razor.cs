using Auth.UI.Components.Pages;
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

        protected string Mode { get; set; } = "login";
        protected RegisterRequest RegisterModel { get; set; } = new();
        protected LoginRequest LoginModel { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected bool RequiresFido2 { get; set; }
        protected bool OtpRequested { get; set; }
        protected string OtpCode { get; set; } = string.Empty;
        protected int LoggedInUserId { get; set; }

        protected async Task RegisterAsync()
        {
            StatusMessage = string.Empty;
            RequiresFido2 = false;

            var result = await AuthController.RegisterAsync(RegisterModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Registration failed";

            if (result.Succeeded && result.Data is not null)
            {
                Mode = "login";
                LoginModel.Username = RegisterModel.Username;
                StatusMessage = "Registration successful. You can now login with your credentials and configure a passkey in your profile.";
            }
        }

        protected async Task LoginAsync()
        {
            StatusMessage = string.Empty;
            RequiresFido2 = false;
            OtpRequested = false;

            var result = await AuthController.LoginAsync(LoginModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Login failed";

            if (result.Succeeded)
            {
                if (result.Data?.RequiresFido2 == true)
                {
                    RequiresFido2 = true;
                    LoggedInUserId = result.Data.UserId;
                }
                else
                {
                    NavigationManager.NavigateTo("/profile");
                }
            }
        }

        protected void GoToFido2()
        {
            NavigationManager.NavigateTo($"/fido2/{LoggedInUserId}");
        }

        protected async Task RequestOtpAsync()
        {
            StatusMessage = string.Empty;
            var response = await AuthController.RequestOtpAsync(new OtpRequest { UserId = LoggedInUserId });
            Succeeded = response.Succeeded;
            StatusMessage = response.Message ?? "Failed to request OTP";
            if (response.Succeeded)
            {
                OtpRequested = true;
            }
        }

        protected async Task VerifyOtpAsync()
        {
            StatusMessage = string.Empty;
            var response = await AuthController.VerifyOtpAsync(new OtpVerifyRequest { UserId = LoggedInUserId, Otp = OtpCode });
            Succeeded = response.Succeeded;
            StatusMessage = response.Message ?? "Failed to verify OTP";
            if (response.Succeeded)
            {
                NavigationManager.NavigateTo("/profile");
            }
        }
    }
}
