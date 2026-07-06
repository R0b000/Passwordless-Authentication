namespace Auth.UI.src.Model.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
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
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool RequiresFido2 { get; set; }
    }

    public class UserIdResult
    {
        public int UserId { get; set; }
    }

    public class Fido2ChallengeRequest
    {
        public int UserId { get; set; }
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
    }

    public class Fido2VerifyResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
    }
}
