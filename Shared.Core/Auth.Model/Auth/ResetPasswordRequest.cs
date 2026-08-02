using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Model.Models.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 6)]
        public string Otp { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}