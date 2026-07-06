namespace PasswordlessApi.Api.Models.ResponseModel.Auth
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool RequiresFido2 { get; set; }
    }
}
