using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Auth.UI.Components.Pages
{
    public partial class Fido2 : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        [Parameter] public int? UserIdParam { get; set; }

        protected int UserId { get; set; }
        protected Fido2VerifyRequest VerifyModel { get; set; } = new();
        protected string AssertionOptions { get; set; } = string.Empty;
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected override void OnParametersSet()
        {
            if (UserIdParam.HasValue)
            {
                UserId = UserIdParam.Value;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && UserIdParam.HasValue)
            {
                await StartAssertionAsync();
            }
        }

        protected async Task StartAssertionAsync()
        {
            StatusMessage = string.Empty;
            AssertionOptions = string.Empty;
            VerifyModel = new Fido2VerifyRequest { UserId = UserId };

            var result = await AuthController.CreateFido2ChallengeAsync(UserId);
            Succeeded = result.Succeeded;
            StatusMessage = result.Message ?? "Failed to create challenge";

            if (!result.Succeeded || result.Data is null)
            {
                return;
            }

            AssertionOptions = result.Data.PublicKeyCredentialCreationOptions;

            try
            {
                var jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./webauthn.js");
                var cred = await jsModule.InvokeAsync<WebAuthnAssertion>(
                    "getCredential",
                    result.Data.PublicKeyCredentialCreationOptions,
                    result.Data.Challenge);

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
                Succeeded = false;
                StatusMessage = $"Passkey assertion failed: {ex.Message}";
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
