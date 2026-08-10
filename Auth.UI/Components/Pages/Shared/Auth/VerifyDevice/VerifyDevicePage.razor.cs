using Microsoft.AspNetCore.Components;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Security;

namespace Auth.UI.Components.Pages.Shared.Auth.VerifyDevice
{
    public partial class VerifyDevicePage 
    {

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
            try
            {
                Loader.Show("Verifying device...");
                var result = await SecurityManager.VerifyDeviceAsync(new VerifyDeviceRequest
                {
                    Code = Code,
                    TrustDevice = TrustDevice
                });

                Succeeded = result.Succeeded;
                StatusMessage = result.Messages ?? string.Empty;

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
            finally
            {
                Loader.Hide();
            }
        }
    }
}
