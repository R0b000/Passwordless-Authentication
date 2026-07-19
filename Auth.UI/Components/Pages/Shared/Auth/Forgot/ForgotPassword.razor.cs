using global::Shared.UI.Components.Toaster;
using Microsoft.AspNetCore.Components;
using global::Shared.UI.Manager.Interface.Auth;

namespace Auth.UI.Components.Pages.Shared.Forgot
{
    public partial class ForgotPassword : ComponentBase
    {
        [Inject] private IAccountManager AccountManager { get; set; } = default!;
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

            var result = await AccountManager.RequestPasswordResetAsync(Email);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess(StatusMessage);
            else Toaster.ShowDanger(StatusMessage);
        }
    }
}
