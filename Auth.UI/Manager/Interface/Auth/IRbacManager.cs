using Shared.Data.Wrapper;
using Auth.Model.Models.Common;
using Auth.Model.Models.Rbac;

namespace Auth.UI.Manager.Interface.Auth
{
    public interface IRbacManager
    {
        Task<IResponse<PaginatedResponse<UserRoleResponse>>> GetUsersWithRolesAsync(int page, int pageSize);
        Task<IResponse<PaginatedResponse<RoleDto>>> GetRolesAsync(int page, int pageSize);
        Task<IResponse<PaginatedResponse<PermissionDto>>> GetPermissionsAsync(int page, int pageSize);
        Task<IResponse<bool>> AssignRoleToUserAsync(int userId, int roleId);
        Task<IResponse<bool>> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IResponse<IEnumerable<string>>> GetUserRolesAsync(int userId);
    }
}
