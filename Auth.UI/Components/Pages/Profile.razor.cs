using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Auth.UI.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        protected AuthResponse? CurrentUser { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadProfileAsync();
        }

        protected async Task LoadProfileAsync()
        {
            StatusMessage = string.Empty;
            var result = await AuthController.MeAsync();
            Succeeded = result.Succeeded;
            StatusMessage = result.Succeeded ? string.Empty : (result.Message ?? "Unauthorized");
            CurrentUser = result.Succeeded ? result.Data : null;
        }

        protected async Task RegisterPasskeyAsync(int userId, string username)
        {
            StatusMessage = string.Empty;
            var optionsResult = await AuthController.RequestAttestationOptionsAsync(
                new Fido2AttestationOptionsRequest { UserId = userId, Username = username });

            if (!optionsResult.Succeeded || optionsResult.Data is null)
            {
                StatusMessage = optionsResult.Message ?? "Failed to get attestation options";
                Succeeded = false;
                return;
            }

            try
            {
                var jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./webauthn.js");

                var cred = await jsModule.InvokeAsync<WebAuthnCredential>(
                    "createCredential",
                    optionsResult.Data.PublicKeyCredentialCreationOptions,
                    optionsResult.Data.Challenge);

                var fullResponse = new
                {
                    id = cred.id,
                    rawId = cred.rawId,
                    type = cred.type,
                    response = cred.response
                };

                var registerResult = await AuthController.RegisterCredentialAsync(
                    new Fido2RegisterRequest
                    {
                        UserId = userId,
                        Username = username,
                        AttestationResponse = System.Text.Json.JsonSerializer.Serialize(fullResponse),
                        AttestationChallenge = optionsResult.Data.Challenge,
                        Transports = cred.transports is null ? string.Empty : string.Join(",", cred.transports)
                    });

                Succeeded = registerResult.Succeeded;
                StatusMessage = registerResult.Data?.Message ?? registerResult.Message ?? (registerResult.Succeeded ? "Passkey registered successfully" : "Passkey registration failed");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Passkey registration failed: {ex.Message}";
                Succeeded = false;
            }
        }
    }
}
