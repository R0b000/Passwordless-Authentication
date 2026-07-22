using global::Shared.UI.Components.Modal;
using global::Shared.UI.Components.Toaster;
using Microsoft.AspNetCore.Components;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Account;

namespace Auth.UI.Components.Pages.Customer.Account.Privacy
{
    public partial class Privacy_Page : ComponentBase
    {
        [Inject] private IAccountManager AccountManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected PrivacySettings Settings { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        private ConfirmationModal _confirm = default!;

        protected override async Task OnInitializedAsync()
        {
            var result = await AccountManager.GetPrivacyAsync();
            Settings = (result.Succeeded && result.Data is not null) ? result.Data : new PrivacySettings();
        }

        protected async Task SaveAsync()
        {
            var result = await AccountManager.UpdatePrivacyAsync(Settings);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess("Privacy preferences updated");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected async Task DownloadAsync()
        {
            var result = await AccountManager.DownloadDataAsync();
            if (result.Succeeded)
                Toaster.ShowSuccess("Your data export is being prepared (demo)");
            else
                Toaster.ShowDanger(result.Messages ?? "Download failed");
        }

        protected async Task OpenDelete() => await _confirm.ShowAsync();

        protected async Task ConfirmDeleteAsync()
        {
            var result = await AccountManager.DeleteAccountAsync();
            if (result.Succeeded) Toaster.ShowSuccess("Account scheduled for deletion (demo)");
            else Toaster.ShowDanger(result.Messages ?? "Deletion failed");
        }
    }
}


