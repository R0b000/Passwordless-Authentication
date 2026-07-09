using System.ComponentModel.DataAnnotations;

namespace PasswordlessApi.Api.Models.RequestModel.Auth
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
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class OtpRequest
    {
        [Required]
        public int UserId { get; set; }
    }

    public class OtpVerifyRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Otp { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
