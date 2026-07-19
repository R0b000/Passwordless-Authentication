using global::Shared.Core.UIModels.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using global::Shared.UI.Manager.Interface.Auth;

namespace Auth.UI.Components.Pages.Shared.Passkey
{
    public partial class PasskeySetup : ComponentBase
    {
        public enum SetupState { Idle, Processing, Success, Error }

        [Inject] private IAuthManager AuthManager { get; set; } = default!;
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
        private Fido2ChallengeResponse? _attestationOptions;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _webAuthnModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./webauthn.js");

                // Pre-fetch the attestation options so the WebAuthn prompt can run inside the
                // user gesture that triggers StartRegistrationAsync. Any awaited work between
                // the click and navigator.credentials.create() causes Firefox/Blazor to drop
                // document focus/activation, which rejects the ceremony ("page does not have focus").
                _ = LoadAttestationOptionsAsync();
            }
        }

        private async Task LoadAttestationOptionsAsync()
        {
            try
            {
                var origin = new Uri(NavigationManager.BaseUri).GetLeftPart(UriPartial.Authority);
                var result = await AuthManager.RequestAttestationOptionsAsync(new Fido2AttestationOptionsRequest
                {
                    UserId = UserId,
                    Username = Username,
                    Origin = origin
                });

                if (result.Succeeded && result.Data is not null)
                {
                    _attestationOptions = result.Data;
                }
            }
            catch
            {
                // Non-critical: the click handler falls back to a fresh request.
                _attestationOptions = null;
            }
        }

        protected async Task StartRegistrationAsync()
        {
            if (_webAuthnModule is null)
            {
                State = SetupState.Error;
                StatusMessage = "The passkey module is still loading. Please try again.";
                return;
            }

            State = SetupState.Processing;
            StatusDetail = "Contacting server...";

            var origin = new Uri(NavigationManager.BaseUri).GetLeftPart(UriPartial.Authority);

            // Options are normally pre-fetched; fall back to a synchronous request only if missing.
            if (_attestationOptions is null)
            {
                await LoadAttestationOptionsAsync();
            }

            if (_attestationOptions is null)
            {
                State = SetupState.Error;
                StatusMessage = "Failed to get registration options. Please try again.";
                return;
            }

            StatusDetail = "Waiting for device...";
            try
            {
                // Call JS to create the credential. With options already loaded there is no
                // awaited work between this user gesture and navigator.credentials.create().
                var cred = await _webAuthnModule.InvokeAsync<WebAuthnCredential>(
                    "createCredential",
                    _attestationOptions.PublicKeyCredentialCreationOptions,
                    _attestationOptions.Challenge);

                StatusDetail = "Verifying with server...";
                var verifyResult = await AuthManager.RegisterCredentialAsync(new Fido2RegisterRequest
                {
                    UserId = UserId,
                    Username = Username,
                    AttestationResponse = JsonSerializer.Serialize(cred),
                    AttestationChallenge = _attestationOptions.Challenge,
                    Transports = string.Join(",", cred.transports ?? new List<string>()),
                    Origin = origin
                });

                if (verifyResult.Succeeded)
                {
                    State = SetupState.Success;
                }
                else
                {
                    _ = LoadAttestationOptionsAsync();
                    State = SetupState.Error;
                    StatusMessage = verifyResult.Messages ?? "Registration failed.";
                }
            }
            catch (Exception ex)
            {
                // Refresh for the next attempt (the challenge is now consumed/expired).
                _ = LoadAttestationOptionsAsync();
                State = SetupState.Error;
                StatusMessage = $"Device error: {ex.Message}";
            }
        }

        protected void SkipSetup() => OnSkipped.InvokeAsync();
        protected void FinishSetup() => OnCompleted.InvokeAsync();
    }
}
