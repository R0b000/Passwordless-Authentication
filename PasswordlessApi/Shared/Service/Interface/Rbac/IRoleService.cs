using API.Shared.Models.DTOs.Rbac;
using API.Shared.Models.Entities;

namespace API.Shared.Service.Interface.Rbac
{
    public interface IRoleService
    {
        Task<Role?> CreateRoleAsync(string name, string? description);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByNameAsync(string name);
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<RoleDto?> GetRoleWithPermissionsAsync(int roleId);
        Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> IsSystemRoleAsync(int roleId);
    }
}
