namespace PasswordlessApi.Api.Models.ResponseModel.Auth
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? Token { get; set; }
    }
}
