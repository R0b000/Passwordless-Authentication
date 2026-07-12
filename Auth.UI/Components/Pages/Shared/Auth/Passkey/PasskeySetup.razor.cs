using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Auth.UI.Components.Pages.Shared.Passkey
{
    public partial class PasskeySetup : ComponentBase
    {
        public enum SetupState { Idle, Processing, Success, Error }

        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        [Parameter] public int UserId { get; set; }
        [Parameter] public string Username { get; set; } = string.Empty;
        [Parameter] public EventCallback OnCompleted { get; set; }
        [Parameter] public EventCallback OnSkipped { get; set; }

        protected SetupState State { get; set; } = SetupState.Idle;
        protected string StatusMessage { get; set; } = string.Empty;
        protected string StatusDetail { get; set; } = string.Empty;
        private IJSObjectReference? _webAuthnModule;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _webAuthnModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./webauthn.js");
            }
        }

        protected async Task StartRegistrationAsync()
        {
            State = SetupState.Processing;
            StatusDetail = "Contacting server...";

            var origin = new Uri(NavigationManager.BaseUri).GetLeftPart(UriPartial.Authority);
            var optionsResult = await AuthController.RequestAttestationOptionsAsync(new Fido2AttestationOptionsRequest
            {
                UserId = UserId,
                Username = Username,
                Origin = origin
            });

            if (!optionsResult.Succeeded || optionsResult.Data == null)
            {
                State = SetupState.Error;
                StatusMessage = optionsResult.Message ?? "Failed to get registration options.";
                return;
            }

            StatusDetail = "Waiting for device...";
            try
            {
                // Call JS to create credential
                var cred = await _webAuthnModule!.InvokeAsync<WebAuthnCredential>(
                    "createCredential",
                    optionsResult.Data.PublicKeyCredentialCreationOptions,
                    optionsResult.Data.Challenge);

                StatusDetail = "Verifying with server...";
                var verifyResult = await AuthController.RegisterCredentialAsync(new Fido2RegisterRequest
                {
                    UserId = UserId,
                    Username = Username,
                    AttestationResponse = JsonSerializer.Serialize(cred.response),
                    AttestationChallenge = optionsResult.Data.Challenge,
                    Transports = string.Join(",", cred.transports ?? new List<string>()),
                    Origin = origin
                });

                if (verifyResult.Succeeded)
                {
                    State = SetupState.Success;
                }
                else
                {
                    State = SetupState.Error;
                    StatusMessage = verifyResult.Message ?? "Registration failed.";
                }
            }
            catch (Exception ex)
            {
                State = SetupState.Error;
                StatusMessage = $"Device error: {ex.Message}";
            }
        }

        protected void SkipSetup() => OnSkipped.InvokeAsync();
        protected void FinishSetup() => OnCompleted.InvokeAsync();
    }
}