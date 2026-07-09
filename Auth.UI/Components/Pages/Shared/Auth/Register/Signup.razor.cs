using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Shared.Register
{
    public partial class Signup : ComponentBase
    {
        [Inject] private AccountController AccountController { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        protected RegisterRequest Model { get; set; } = new();
        protected string ConfirmPassword { get; set; } = string.Empty;
        protected bool AcceptTerms { get; set; }
        protected bool ShowPassword { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected async Task SubmitAsync()
        {
            if (string.IsNullOrWhiteSpace(Model.Username) || string.IsNullOrWhiteSpace(Model.Email) || string.IsNullOrWhiteSpace(Model.Password))
            {
                Succeeded = false;
                StatusMessage = "Please complete all fields.";
                return;
            }

            if (Model.Password != ConfirmPassword)
            {
                Succeeded = false;
                StatusMessage = "Passwords do not match.";
                return;
            }

            if (!AcceptTerms)
            {
                Succeeded = false;
                StatusMessage = "You must accept the Terms & Conditions.";
                return;
            }

            var result = await AccountController.RegisterAsync(Model);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;

            if (result.Succeeded)
            {
                Toaster.ShowSuccess("Account created. Please sign in.");
                Navigation.NavigateTo("/login");
            }
            else
            {
                Toaster.ShowDanger(StatusMessage);
            }
        }
    }
}
