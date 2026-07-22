using Auth.Model.Models.Entities;
using Auth.Model.Wrapper;

namespace Auth.API.Service.Interface.Rbac
{
    public interface IUserRoleService
    {
        Task<IResponse<bool>> AssignRoleToUserAsync(int userId, int roleId);
        Task<IResponse<bool>> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IResponse<IEnumerable<UserRole>>> GetUserRolesAsync(int userId);
        Task<IResponse<IEnumerable<string>>> GetUserRoleNamesAsync(int userId);
        Task<IResponse<IEnumerable<string>>> GetUserPermissionNamesAsync(int userId);
        Task<IResponse<User?>> GetUserWithRolesAndPermissionsAsync(int userId);
        Task<IResponse<bool>> HasPermissionAsync(int userId, string permissionName);
        Task<IResponse<bool>> IsInRoleAsync(int userId, string roleName);
        Task<IResponse<IEnumerable<User>>> GetUsersInRoleAsync(string roleName);
        Task<IResponse<IEnumerable<User>>> GetAllUsersWithRolesAsync();
        int? GetCurrentUserId();
    }
}

