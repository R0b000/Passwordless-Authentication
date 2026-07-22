using global::Shared.UI.Components.Toaster;
using global::Auth.Model.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Account;

namespace Auth.UI.Components.Pages.Customer.Account.Profile
{
    public partial class Profile_Page : ComponentBase
    {
        [Inject] private IAccountManager AccountManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;
        [Inject] private ITokenStore TokenStore { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        protected UserProfile? Model { get; set; }
        protected bool EditMode { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Model?.DisplayName)) return "?";
                var parts = Model.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var letters = parts.Length > 1
                    ? parts[0][0].ToString() + parts[^1][0]
                    : parts[0][0].ToString();
                return letters.ToUpperInvariant();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var result = await AccountManager.GetProfileAsync();
            Model = result.Succeeded ? result.Data : new UserProfile();
        }

        protected void EnterEdit() => EditMode = true;

        protected void CancelEdit()
        {
            EditMode = false;
            _ = ReloadAsync();
        }

        protected async Task ReloadAsync()
        {
            var result = await AccountManager.GetProfileAsync();
            if (result.Succeeded) Model = result.Data;
        }

        protected async Task SaveAsync()
        {
            if (Model is null) return;
            var result = await AccountManager.UpdateProfileAsync(Model);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            EditMode = !result.Succeeded;
            if (result.Succeeded) Toaster.ShowSuccess("Profile updated");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected void OnAvatarSelected(IBrowserFile file)
        {
            Toaster.ShowInfo("Profile picture updated (demo)");
        }

        protected void Logout()
        {
            TokenStore.Clear();
            Navigation.NavigateTo("/login", replace: true);
        }
    }
}



