using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Account;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Customer
{
    public partial class AccountSettingsPage : ComponentBase
    {
        [Inject] private AccountController AccountController { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected AccountSettings? Settings { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await AccountController.GetSettingsAsync();
            Settings = result.Succeeded ? result.Data : new AccountSettings();
        }

        protected async Task SaveAsync()
        {
            if (Settings is null) return;
            var result = await AccountController.UpdateSettingsAsync(Settings);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;
            if (result.Succeeded) Toaster.ShowSuccess("Settings saved");
            else Toaster.ShowDanger(StatusMessage);
        }

        protected async Task ResetAsync()
        {
            var result = await AccountController.GetSettingsAsync();
            if (result.Succeeded) Settings = result.Data;
            StatusMessage = string.Empty;
        }
    }
}
