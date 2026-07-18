namespace Shared.Core.Models.RequestModel.Auth
{
    public class Fido2RegisterRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AttestationResponse { get; set; } = string.Empty;
        public string AttestationChallenge { get; set; } = string.Empty;
        public string Transports { get; set; } = string.Empty;
        public string? Origin { get; set; }
    }
}
