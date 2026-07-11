namespace PasswordlessApi.Api.Models.RequestModel.Auth
{
    public class Fido2ChallengeRequest
    {
        public int UserId { get; set; }
        public string? Origin { get; set; }
    }
}
