namespace Auth.Model.Models.Entities
{
    public class UserCredential
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string CredentialId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public long SignCount { get; set; }
        public string? Transports { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

