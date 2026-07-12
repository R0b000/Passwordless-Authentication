namespace PasswordlessApi.Api.Models.ResponseModel.Auth
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool RequiresFido2 { get; set; }
        public bool RequiresFido2Registration { get; set; }
        public string? Role { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class OtpResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Otp { get; set; }
    }
}
