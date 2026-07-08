namespace PasswordlessApi.Api.Models.RequestModel.Auth
{
    public class Fido2AttestationOptionsRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
