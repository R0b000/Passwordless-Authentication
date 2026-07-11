namespace PasswordlessApi.Api.Models.ResponseModel.Auth
{
    public class Fido2VerifyResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
    }
}
