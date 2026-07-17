using API.Shared.Models.DTOs.Rbac;
using API.Shared.Models.Entities;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Rbac
{
    public interface IRoleService
    {
        Task<IResponse<Role?>> CreateRoleAsync(string name, string? description);
        Task<IResponse<IEnumerable<Role>>> GetAllRolesAsync();
        Task<IResponse<Role?>> GetRoleByNameAsync(string name);
        Task<IResponse<Role?>> GetRoleByIdAsync(int roleId);
        Task<IResponse<RoleDto?>> GetRoleWithPermissionsAsync(int roleId);
        Task<IResponse<bool>> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<IResponse<bool>> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<IResponse<bool>> DeleteRoleAsync(int roleId);
        Task<IResponse<bool>> IsSystemRoleAsync(int roleId);
    }
}
