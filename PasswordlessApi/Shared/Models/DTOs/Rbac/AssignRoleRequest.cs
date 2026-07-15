namespace API.Shared.Models.DTOs.Rbac
{
    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }
}
