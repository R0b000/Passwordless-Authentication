using Auth.UI.src.Manager.Routing;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Shared.Register
{
    public partial class Signup : ComponentBase
    {
        [Inject] private IAccountManager AccountManager { get; set; } = default!;
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

            if (Model.Username.Length < 3)
            {
                Succeeded = false;
                StatusMessage = "Username must be at least 3 characters.";
                return;
            }

            if (!IsValidEmail(Model.Email))
            {
                Succeeded = false;
                StatusMessage = "Enter a valid email address.";
                return;
            }

            if (Model.Password.Length < 8 || Model.Password.Length > 128)
            {
                Succeeded = false;
                StatusMessage = "Password must be between 8 and 128 characters.";
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

            var result = await AccountManager.RegisterAsync(Model);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;

            if (result.Succeeded)
            {
                Toaster.ShowSuccess("Account created. Please sign in.");
                Navigation.NavigateTo(AuthRoute.Login);
            }
            else
            {
                Toaster.ShowDanger(StatusMessage);
            }
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email)
                && email.Length <= 300
                && System.Text.RegularExpressions.Regex.IsMatch(
                    email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}
