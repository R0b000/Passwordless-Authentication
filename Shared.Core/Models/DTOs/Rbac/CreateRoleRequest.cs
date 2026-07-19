namespace Shared.Core.Models.DTOs.Rbac
{
    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> PermissionNames { get; set; } = new();
    }
}
