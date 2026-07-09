using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace Auth.UI.Components.Pages.Shared
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

        protected string Email { get; set; } = string.Empty;
        protected bool IsEmailResolved { get; set; }
        protected bool IsResolving { get; set; }
        protected string? ResolvedUsername { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        private IJSObjectReference? _webAuthnModule;

        protected override void OnParametersSet()
        {
            if (UserIdParam.HasValue)
            {
                UserId = UserIdParam.Value;
                IsEmailResolved = true;
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

        protected async Task ResolveAccountAsync()
        {
            StatusMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(Email))
            {
                StatusMessage = "Please enter your email address.";
                Succeeded = false;
                return;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                StatusMessage = "Please enter a valid email address.";
                Succeeded = false;
                return;
            }

            IsResolving = true;
            State = PasskeyState.Requesting;
            StatusDetail = "Resolving your account…";

            try
            {
                var result = await AuthController.GetUserByEmailAsync(Email);
                if (result.Succeeded && result.Data is not null)
                {
                    UserId = result.Data.UserId;
                    ResolvedUsername = result.Data.Username;
                    IsEmailResolved = true;
                    State = PasskeyState.Idle;
                    StatusDetail = string.Empty;
                    Succeeded = true;
                }
                else
                {
                    State = PasskeyState.Idle;
                    StatusMessage = result.Message ?? "No account found with this email address.";
                    Succeeded = false;
                }
            }
            catch (Exception ex)
            {
                State = PasskeyState.Idle;
                StatusMessage = $"An error occurred during account resolution: {ex.Message}";
                Succeeded = false;
            }
            finally
            {
                IsResolving = false;
            }
        }

        protected void ResetEmailLookup()
        {
            Email = string.Empty;
            IsEmailResolved = false;
            UserId = 0;
            ResolvedUsername = null;
            StatusMessage = string.Empty;
            State = PasskeyState.Idle;
            StatusDetail = string.Empty;
        }

        protected async Task StartAssertionAsync(string? authenticatorAttachment = null)
        {
            if (UserId <= 0)
            {
                State = PasskeyState.Error;
                StatusDetail = "Account resolution required to continue. Please go back and enter your email.";
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
                    new { authenticatorAttachment = authenticatorAttachment, userVerification = "required" });

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
}
