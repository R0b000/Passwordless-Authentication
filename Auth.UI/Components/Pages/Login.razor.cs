using Auth.UI.Components.Pages;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;

        protected string Mode { get; set; } = "login";
        protected RegisterRequest RegisterModel { get; set; } = new();
        protected LoginRequest LoginModel { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected bool RequiresFido2 { get; set; }

        protected async Task RegisterAsync()
        {
            StatusMessage = string.Empty;
            RequiresFido2 = false;

            var result = await AuthController.RegisterAsync(RegisterModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Registration failed";

            if (result.Succeeded)
            {
                Mode = "login";
                LoginModel.Username = RegisterModel.Username;
            }
        }

        protected async Task LoginAsync()
        {
            StatusMessage = string.Empty;
            RequiresFido2 = false;

            var result = await AuthController.LoginAsync(LoginModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Login failed";

            if (result.Succeeded && result.Data?.RequiresFido2 == true)
            {
                RequiresFido2 = true;
            }
        }

        protected void GoToFido2()
        {
            NavigationManager.NavigateTo("/fido2");
        }

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    }
}
