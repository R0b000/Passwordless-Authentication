using global::Shared.UI.Components.Toaster;
using Microsoft.AspNetCore.Components;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Security;

namespace Auth.UI.Components.Pages.Customer.Account.Sessions
{
    public partial class Sessions_Page : ComponentBase
    {
        [Inject] private ISecurityManager SecurityManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected List<SessionInfo> SessionItems { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            var result = await SecurityManager.GetSessionsAsync();
            SessionItems = result.Succeeded ? result.Data ?? new List<SessionInfo>() : new List<SessionInfo>();
        }

        protected string DeviceIcon(string deviceType) => deviceType.ToLowerInvariant() switch
        {
            "mobile" => "smartphone",
            "tablet" => "usb",
            _ => "usb"
        };

        protected async Task RevokeAsync(string id)
        {
            var result = await SecurityManager.RevokeSessionAsync(id);
            Toaster.Show(result.Messages ?? "Done", result.Succeeded ? ToastType.Success : ToastType.Danger);
            await ReloadAsync();
        }

        protected async Task RevokeOthersAsync()
        {
            var result = await SecurityManager.RevokeAllSessionsAsync(false);
            Toaster.Show(result.Messages ?? "Done", result.Succeeded ? ToastType.Success : ToastType.Danger);
            await ReloadAsync();
        }

        protected async Task RevokeAllAsync()
        {
            var result = await SecurityManager.RevokeAllSessionsAsync(true);
            Toaster.Show(result.Messages ?? "Done", result.Succeeded ? ToastType.Success : ToastType.Danger);
            await ReloadAsync();
        }
    }
}


