using Auth.UI.src.Manager.Routing;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Service.Interface;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Shared.Reset
{
    public partial class ResetPassword : ComponentBase
    {
        [Inject] private IAccountManager AccountManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string Token { get; set; } = string.Empty;

        protected string NewPassword { get; set; } = string.Empty;
        protected string ConfirmPassword { get; set; } = string.Empty;
        protected bool ShowPassword { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected async Task SubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(NewPassword))
            {
                Succeeded = false;
                StatusMessage = "Please enter the reset code and a new password.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                Succeeded = false;
                StatusMessage = "Passwords do not match.";
                return;
            }

            var result = await AccountManager.ResetPasswordAsync(Token, NewPassword);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;

            if (result.Succeeded)
            {
                Toaster.ShowSuccess("Password reset. Please sign in.");
                Navigation.NavigateTo(AuthRoute.Login);
            }
            else
            {
                Toaster.ShowDanger(StatusMessage);
            }
        }
    }
}
