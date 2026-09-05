using Microsoft.AspNetCore.Components.Forms;
using global::Shared.UI.Components.Upload;
using global::Shared.UI.Components.Tag;
using Auth.Model.Models.Account;
using global::Auth.Model.Models.Auth;

namespace Auth.UI.Components.Pages.Customer.Account.Profile
{
    public partial class ProfilePage 
    {

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

        protected override Task OnInitializedAsync()
        {
            Model = new UserProfile();
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ReloadAsync();
                StateHasChanged();
            }
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

        protected bool LogoutModalVisible { get; set; }

        protected void Logout()
        {
            LogoutModalVisible = true;
        }

        protected async Task ConfirmLogout()
        {
            await TokenStore.Clear();
            Navigation.NavigateTo("/login", replace: true, forceLoad: true);
        }
    }
}



