namespace API.Shared.Models.DTOs.Rbac
{
    public class UpdateRoleRequest
    {
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string>? PermissionNames { get; set; }
    }
}
