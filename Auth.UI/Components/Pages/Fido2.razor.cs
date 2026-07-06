using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages
{
    public partial class Fido2 : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        protected int UserId { get; set; }
        protected Fido2VerifyRequest VerifyModel { get; set; } = new();
        protected string AssertionOptions { get; set; } = string.Empty;
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected async Task StartAssertionAsync()
        {
            StatusMessage = string.Empty;
            AssertionOptions = string.Empty;
            VerifyModel = new Fido2VerifyRequest { UserId = UserId };

            var result = await AuthController.CreateFido2ChallengeAsync(UserId);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? "Failed to create challenge";

            if (result.Succeeded && result.Data is not null)
            {
                AssertionOptions = result.Data.PublicKeyCredentialCreationOptions;
                VerifyModel.CredentialId = result.Data.Id;
            }
        }

        protected async Task VerifyAsync()
        {
            StatusMessage = string.Empty;
            VerifyModel.UserId = UserId;

            var result = await AuthController.VerifyFido2AssertionAsync(VerifyModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Verification failed";

            if (result.Succeeded)
            {
                NavigationManager.NavigateTo("/profile");
            }
        }
    }
}
