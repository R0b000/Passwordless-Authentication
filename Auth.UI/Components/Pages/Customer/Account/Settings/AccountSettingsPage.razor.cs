using global::Shared.UI.Components.Toaster;
using Microsoft.AspNetCore.Components;
using global::Shared.UI.Manager.Interface.Auth;
using Auth.UI.Models.Account;

namespace Auth.UI.Components.Pages.Customer.Account.Settings
{
    public partial class AccountSettingsPage : ComponentBase
    {
        [Inject] private IAccountManager AccountManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected AccountSettings? Settings { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await AccountManager.GetSettingsAsync();
            Settings = result.Succeeded ? result.Data : new AccountSettings();
        }

        protected async Task SaveAsync()
        {
            if (Settings is null) return;
            var result = await AccountManager.UpdateSettingsAsync(Settings);
            Succeeded = result.Succeeded;
            StatusMessage = result.Messages ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess("Settings saved");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected async Task ResetAsync()
        {
            var result = await AccountManager.GetSettingsAsync();
            if (result.Succeeded) Settings = result.Data;
            StatusMessage = string.Empty;
        }
    }
}
