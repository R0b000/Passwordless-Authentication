namespace Shared.Core.Models.DTOs.Rbac
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public List<string> Permissions { get; set; } = new();
        public int UserCount { get; set; }
    }
}
