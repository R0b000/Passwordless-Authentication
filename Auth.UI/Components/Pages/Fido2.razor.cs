using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Auth.UI.Components.Pages
{
    public partial class Fido2 : ComponentBase
    {
        public enum PasskeyState { Idle, Requesting, Awaiting, Verifying, Success, Error }

        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        [Parameter] public int? UserIdParam { get; set; }

        protected PasskeyState State { get; set; } = PasskeyState.Idle;
        protected int UserId { get; set; }
        protected Fido2VerifyRequest VerifyModel { get; set; } = new();
        protected string AssertionOptions { get; set; } = string.Empty;
        protected string StatusDetail { get; set; } = string.Empty;

        private IJSObjectReference? _webAuthnModule;

        protected override void OnParametersSet()
        {
            if (UserIdParam.HasValue)
            {
                UserId = UserIdParam.Value;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _webAuthnModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./webauthn.js");

                if (UserIdParam.HasValue)
                {
                    await StartAssertionAsync();
                }
            }
        }

        protected async Task StartAssertionAsync(string? authenticatorAttachment = null)
        {
            if (UserId <= 0)
            {
                State = PasskeyState.Error;
                StatusDetail = "Enter your account ID to continue, or go back and sign in with your password.";
                return;
            }

            State = PasskeyState.Requesting;
            StatusDetail = "Contacting the server to prepare your passkey challenge…";

            var result = await AuthController.CreateFido2ChallengeAsync(UserId);
            if (!result.Succeeded || result.Data is null)
            {
                State = PasskeyState.Error;
                StatusDetail = result.Message ?? "Unable to start passkey sign-in. Please try again.";
                return;
            }

            AssertionOptions = result.Data.PublicKeyCredentialCreationOptions;
            VerifyModel = new Fido2VerifyRequest { UserId = UserId };

            State = PasskeyState.Awaiting;
            StatusDetail = authenticatorAttachment == "cross-platform"
                ? "Insert your security key and tap it when it blinks."
                : "Use your fingerprint, face, or screen lock on this device.";

            try
            {
                var cred = await _webAuthnModule!.InvokeAsync<WebAuthnAssertion>(
                    "getCredential",
                    result.Data.PublicKeyCredentialCreationOptions,
                    result.Data.Challenge,
                    new { authenticatorAttachment = authenticatorAttachment, userVerification = "preferred" });

                VerifyModel.Challenge = cred.challenge;
                VerifyModel.CredentialId = cred.id;
                VerifyModel.ClientDataJson = cred.response.clientDataJSON;
                VerifyModel.AuthenticatorData = cred.response.authenticatorData;
                VerifyModel.Signature = cred.response.signature;
                VerifyModel.UserId = UserId;

                await VerifyAsync();
            }
            catch (Exception ex)
            {
                State = PasskeyState.Error;
                StatusDetail = await MapErrorAsync(ex);
            }
        }

        protected async Task VerifyAsync()
        {
            State = PasskeyState.Verifying;
            StatusDetail = "Verifying your passkey with the server…";

            var result = await AuthController.VerifyFido2AssertionAsync(VerifyModel);
            if (result.Succeeded)
            {
                State = PasskeyState.Success;
                StatusDetail = result.Data?.Message ?? "You're signed in.";
                NavigationManager.NavigateTo("/profile");
                return;
            }

            State = PasskeyState.Error;
            StatusDetail = result.Data?.Message ?? result.Message ?? "The passkey could not be verified. Please try again.";
        }

        private async Task<string> MapErrorAsync(Exception ex)
        {
            try
            {
                return await _webAuthnModule!.InvokeAsync<string>("describeWebAuthnError", ex);
            }
            catch
            {
                return ex.Message;
            }
        }
    }

    public class WebAuthnAssertion
    {
        [JsonPropertyName("id")] public string id { get; set; } = string.Empty;
        [JsonPropertyName("rawId")] public string rawId { get; set; } = string.Empty;
        [JsonPropertyName("type")] public string type { get; set; } = string.Empty;
        [JsonPropertyName("challenge")] public string challenge { get; set; } = string.Empty;
        [JsonPropertyName("response")] public WebAuthnAssertionResponse response { get; set; } = new();
    }

    public class WebAuthnAssertionResponse
    {
        [JsonPropertyName("clientDataJSON")] public string clientDataJSON { get; set; } = string.Empty;
        [JsonPropertyName("authenticatorData")] public string authenticatorData { get; set; } = string.Empty;
        [JsonPropertyName("signature")] public string signature { get; set; } = string.Empty;
        [JsonPropertyName("userHandle")] public string? userHandle { get; set; }
    }
}
