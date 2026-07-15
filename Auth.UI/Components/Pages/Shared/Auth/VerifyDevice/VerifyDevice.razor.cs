using Auth.UI.Shared.Components.Toaster;
using Auth.UI.Shared.Manager.Controller;
using Auth.UI.src.Model.Security;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages.Shared.VerifyDevice
{
    public partial class VerifyDevice : ComponentBase
    {
        [Inject] private SecurityController SecurityController { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        protected string Code { get; set; } = string.Empty;
        protected bool TrustDevice { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected void SendCodeAsync()
        {
            Toaster.ShowInfo("A new verification code has been sent (demo)");
        }

        protected async Task VerifyAsync()
        {
            var result = await SecurityController.VerifyDeviceAsync(new VerifyDeviceRequest
            {
                Code = Code,
                TrustDevice = TrustDevice
            });

            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? string.Empty;

            if (result.Succeeded)
            {
                Toaster.ShowSuccess(StatusMessage);
                Navigation.NavigateTo("/");
            }
            else
            {
                Toaster.ShowDanger(StatusMessage);
            }
        }
    }
}
