using Auth.UI.Components.UI.Modal;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Account;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Customer.Account.Privacy
{
    public partial class Privacy : ComponentBase
    {
        [Inject] private AccountController AccountController { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected PrivacySettings Settings { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        private ConfirmationModal _confirm = default!;

        protected override async Task OnInitializedAsync()
        {
            var result = await AccountController.GetPrivacyAsync();
            Settings = (result.Succeeded && result.Data is not null) ? result.Data : new PrivacySettings();
        }

        protected async Task SaveAsync()
        {
            var result = await AccountController.UpdatePrivacyAsync(Settings);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess("Privacy preferences updated");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected async Task DownloadAsync()
        {
            var result = await AccountController.DownloadDataAsync();
            if (result.Succeeded)
                Toaster.ShowSuccess("Your data export is being prepared (demo)");
            else
                Toaster.ShowDanger(result.Message ?? "Download failed");
        }

        protected async Task OpenDelete() => await _confirm.ShowAsync();

        protected async Task ConfirmDeleteAsync()
        {
            var result = await AccountController.DeleteAccountAsync();
            if (result.Succeeded) Toaster.ShowSuccess("Account scheduled for deletion (demo)");
            else Toaster.ShowDanger(result.Message ?? "Deletion failed");
        }
    }
}
