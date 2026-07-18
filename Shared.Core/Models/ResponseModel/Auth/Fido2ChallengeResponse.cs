namespace Shared.Core.Models.ResponseModel.Auth
{
    public class Fido2ChallengeResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Challenge { get; set; } = string.Empty;
        public string PublicKeyCredentialCreationOptions { get; set; } = string.Empty;
    }
}
