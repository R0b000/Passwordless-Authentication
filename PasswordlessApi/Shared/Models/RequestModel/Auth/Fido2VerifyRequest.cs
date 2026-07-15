namespace API.Shared.Models.RequestModel.Auth
{
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
}
