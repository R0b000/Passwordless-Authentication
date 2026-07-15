namespace API.Shared.Models.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
