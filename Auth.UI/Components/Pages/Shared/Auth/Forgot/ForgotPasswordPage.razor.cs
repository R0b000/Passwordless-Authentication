using Microsoft.AspNetCore.Components;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Auth;

namespace Auth.UI.Components.Pages.Shared.Auth.Forgot
{
    public partial class ForgotPasswordPage 
    {

        protected ForgotPasswordRequest RequestModel { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected bool IsSubmitting { get; set; }

        protected async Task SubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(RequestModel.Email))
            {
                Succeeded = false;
                StatusMessage = "Please enter your email address.";
                return;
            }

            IsSubmitting = true;
            var result = await AccountManager.RequestPasswordResetAsync(RequestModel);
            IsSubmitting = false;

            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess(StatusMessage);
            else Toaster.ShowDanger(StatusMessage);
        }
    }
}