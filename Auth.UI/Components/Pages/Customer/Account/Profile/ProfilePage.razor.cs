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

        protected override async Task OnInitializedAsync()
        {
            var result = await AccountManager.GetProfileAsync();
            Model = result.Succeeded ? result.Data : new UserProfile();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && (Model is null || string.IsNullOrEmpty(Model.Username)))
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

        protected async Task OnAvatarSelected(IBrowserFile file)
        {
            if (file is null || Model is null) return;
            try
            {
                var format = file.ContentType;
                if (string.IsNullOrEmpty(format)) format = "image/png";
                var resizedFile = await file.RequestImageFileAsync(format, 300, 300);
                using var stream = resizedFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                Model.AvatarUrl = $"data:{format};base64,{base64}";
                Toaster.ShowSuccess("Profile picture updated!");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Toaster.ShowDanger($"Failed to process image: {ex.Message}");
            }
        }

        protected async Task Logout()
        {
await TokenStore.Clear();
            Navigation.NavigateTo("/login", replace: true, forceLoad: true);
        }
    }
}



