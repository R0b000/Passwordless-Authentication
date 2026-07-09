using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Shared.Forgot
{
    public partial class ForgotPassword : ComponentBase
    {
        [Inject] private AccountController AccountController { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected string Email { get; set; } = string.Empty;
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected async Task SubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Succeeded = false;
                StatusMessage = "Please enter your email address.";
                return;
            }

            var result = await AccountController.RequestPasswordResetAsync(Email);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess(StatusMessage);
            else Toaster.ShowDanger(StatusMessage);
        }
    }
}
