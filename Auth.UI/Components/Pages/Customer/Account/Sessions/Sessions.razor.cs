using Auth.UI.Components.UI.Toaster;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Security;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Customer.Account.Sessions
{
    public partial class Sessions : ComponentBase
    {
        [Inject] private SecurityController SecurityController { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected List<SessionInfo> SessionItems { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            var result = await SecurityController.GetSessionsAsync();
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
            var result = await SecurityController.RevokeSessionAsync(id);
            Toaster.Show(result.Message ?? "Done", result.Succeeded ? ToastType.Success : ToastType.Danger);
            await ReloadAsync();
        }

        protected async Task RevokeOthersAsync()
        {
            var result = await SecurityController.RevokeAllSessionsAsync(false);
            Toaster.Show(result.Message ?? "Done", result.Succeeded ? ToastType.Success : ToastType.Danger);
            await ReloadAsync();
        }

        protected async Task RevokeAllAsync()
        {
            var result = await SecurityController.RevokeAllSessionsAsync(true);
            Toaster.Show(result.Message ?? "Done", result.Succeeded ? ToastType.Success : ToastType.Danger);
            await ReloadAsync();
        }
    }
}
