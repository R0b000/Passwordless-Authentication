namespace PasswordlessApi.Api.Models.Entities
{
    public class AuthChallenge
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Challenge { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
