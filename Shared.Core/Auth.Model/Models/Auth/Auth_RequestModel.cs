using System.ComponentModel.DataAnnotations;

namespace Auth.Model.Models.Auth
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class Fido2ChallengeRequest
    {
        public int UserId { get; set; }
        public string? Origin { get; set; }
    }

    public class Fido2AttestationOptionsRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Origin { get; set; }
    }

    public class Fido2RegisterRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AttestationResponse { get; set; } = string.Empty;
        public string AttestationChallenge { get; set; } = string.Empty;
        public string Transports { get; set; } = string.Empty;
        public string? Origin { get; set; }
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
        public string? Origin { get; set; }
    }

    public class OtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class OtpVerifyRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class DeviceInfoRequest
    {
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class RevokeAllSessionsRequest
    {
        public string? Password { get; set; }
    }
}

