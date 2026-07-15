using API.Shared.Models.Entities;
using PasswordlessApi.Api.Models.DTOs.Rbac;

namespace API.Shared.Service.Interface.Rbac
{
    public interface IUserRoleService
    {
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId);
        Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId);
        Task<IEnumerable<string>> GetUserPermissionNamesAsync(int userId);
        Task<User?> GetUserWithRolesAndPermissionsAsync(int userId);
        Task<bool> HasPermissionAsync(int userId, string permissionName);
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName);
        Task<IEnumerable<User>> GetAllUsersWithRolesAsync();
    }
}
