using Auth.UI.Components.Pages;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Auth.UI.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        protected string Mode { get; set; } = "login";
        protected RegisterRequest RegisterModel { get; set; } = new();
        protected LoginRequest LoginModel { get; set; } = new();
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }
        protected bool RequiresFido2 { get; set; }

        protected async Task RegisterAsync()
        {
            StatusMessage = string.Empty;
            RequiresFido2 = false;

            var result = await AuthController.RegisterAsync(RegisterModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Registration failed";

            if (result.Succeeded && result.Data is not null)
            {
                Mode = "login";
                LoginModel.Username = RegisterModel.Username;

                try
                {
                    await RegisterPasskeyAsync(result.Data.UserId, RegisterModel.Username);
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Registered, but passkey setup skipped: {ex.Message}";
                    Succeeded = false;
                }
            }
        }

        protected async Task RegisterPasskeyAsync(int userId, string username)
        {
            var optionsResult = await AuthController.RequestAttestationOptionsAsync(
                new Fido2AttestationOptionsRequest { UserId = userId, Username = username });

            if (!optionsResult.Succeeded || optionsResult.Data is null)
            {
                throw new InvalidOperationException(optionsResult.Message ?? "Failed to get attestation options");
            }

            var jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./webauthn.js");

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

            if (!registerResult.Succeeded)
            {
                throw new InvalidOperationException(registerResult.Data?.Message ?? registerResult.Message ?? "Passkey registration failed");
            }

            StatusMessage = "Registered and passkey created. You can now sign in with your passkey.";
            Succeeded = true;
        }

        protected async Task LoginAsync()
        {
            StatusMessage = string.Empty;
            RequiresFido2 = false;

            var result = await AuthController.LoginAsync(LoginModel);
            Succeeded = result.Succeeded;
            StatusMessage = result.Data?.Message ?? result.Message ?? "Login failed";

            if (result.Succeeded && result.Data?.RequiresFido2 == true)
            {
                RequiresFido2 = true;
            }
        }

        protected void GoToFido2()
        {
            NavigationManager.NavigateTo("/fido2");
        }
    }

    public class WebAuthnCredential
    {
        [JsonPropertyName("id")] public string id { get; set; } = string.Empty;
        [JsonPropertyName("rawId")] public string rawId { get; set; } = string.Empty;
        [JsonPropertyName("type")] public string type { get; set; } = string.Empty;
        [JsonPropertyName("response")] public WebAuthnAttestationResponse response { get; set; } = new();
        [JsonPropertyName("transports")] public List<string>? transports { get; set; }
    }

    public class WebAuthnAttestationResponse
    {
        [JsonPropertyName("clientDataJSON")] public string clientDataJSON { get; set; } = string.Empty;
        [JsonPropertyName("attestationObject")] public string attestationObject { get; set; } = string.Empty;
    }
}
