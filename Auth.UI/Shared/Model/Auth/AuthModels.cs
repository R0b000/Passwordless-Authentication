namespace Auth.UI.Shared.Model.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool RequiresFido2 { get; set; }
        public bool RequiresFido2Registration { get; set; }
    }

    public class UserIdResult
    {
        public int UserId { get; set; }
    }

    public class Fido2ChallengeRequest
    {
        public int UserId { get; set; }
        public string Origin { get; set; } = string.Empty;
    }

    public class Fido2AttestationOptionsRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
    }

    public class Fido2RegisterRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AttestationResponse { get; set; } = string.Empty;
        public string AttestationChallenge { get; set; } = string.Empty;
        public string Transports { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
    }

    public class Fido2ChallengeResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Challenge { get; set; } = string.Empty;
        public string PublicKeyCredentialCreationOptions { get; set; } = string.Empty;
    }

    public class Fido2VerifyRequest
    {
        public int UserId { get; set; }
        public string CredentialId { get; set; } = string.Empty;
        public string ClientDataJson { get; set; } = string.Empty;
        public string AuthenticatorData { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public long? Counter { get; set; }
        public string Challenge { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
    }

    public class Fido2VerifyResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
    }

    public class OtpRequest
    {
        public int UserId { get; set; }
    }

    public class OtpVerifyRequest
    {
        public int UserId { get; set; }
        public string Otp { get; set; } = string.Empty;
    }

    public class OtpResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Otp { get; set; }
    }

    public class WebAuthnCredential
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")] public string id { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("rawId")] public string rawId { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("type")] public string type { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("response")] public WebAuthnAttestationResponse response { get; set; } = new();
        [System.Text.Json.Serialization.JsonPropertyName("transports")] public System.Collections.Generic.List<string>? transports { get; set; }
    }

    public class WebAuthnAttestationResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("clientDataJSON")] public string clientDataJSON { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonPropertyName("attestationObject")] public string attestationObject { get; set; } = string.Empty;
    }
}
