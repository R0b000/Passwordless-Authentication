namespace Auth.Model.Models.Rbac
{
    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> PermissionNames { get; set; } = new();
    }

    public class UpdateRoleRequest
    {
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string>? PermissionNames { get; set; }
    }

    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }

    public class AssignPermissionRequest
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }
}

