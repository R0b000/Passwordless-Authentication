using Auth.UI.Components.UI.Menu;
using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Auth.UI.src.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Auth.UI.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ITokenStore TokenStore { get; set; } = default!;

        protected AuthResponse? CurrentUser { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected bool AccountMenuOpen { get; set; }
        protected string AccountInitial =>
            string.IsNullOrEmpty(CurrentUser?.Username) ? "?" : CurrentUser.Username[0].ToString().ToUpperInvariant();

        protected List<object> MenuItems { get; set; } = new()
        {
            new MenuHeaderItem { Text = "Menu" },
            new MenuLinkItem { Text = "Profile", Url = "/profile", Icon = "user" },
            new MenuLinkItem { Text = "Passkeys", Url = "/fido2", Icon = "fingerprint" },
            new MenuDivider(),
            new MenuLinkItem { Text = "Sign in", Url = "/", Icon = "lock" }
        };

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

        protected void ToggleAccountMenu() => AccountMenuOpen = !AccountMenuOpen;

        protected void CloseAccountMenu() => AccountMenuOpen = false;

        protected void OnMenuAction(MenuActionItem item)
        {
        }

        protected void LogoutAsync()
        {
            TokenStore.Clear();
            AccountMenuOpen = false;
            NavigationManager.NavigateTo("/");
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
