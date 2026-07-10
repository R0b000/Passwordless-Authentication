using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Rbac;

namespace PasswordlessApi.Api.Service.Implementation.Rbac
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ProcedureName = "sp_RBAC";

        public UserRoleService(IDapperRepository dapperRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dapperRepository = dapperRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new { UserRoleAction = "AssignRoleToUser", UserId = userId, RoleId = roleId });

            return result;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new { UserRoleAction = "RemoveRoleFromUser", UserId = userId, RoleId = roleId });

            return result;
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId)
        {
            return await _dapperRepository.QueryAsync<UserRole>(
                ProcedureName,
                new { UserRoleAction = "GetUserRoles", UserId = userId });
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
        {
            return await _dapperRepository.QueryAsync<string>(
                ProcedureName,
                new { UserRoleAction = "GetUserRoleNames", UserId = userId });
        }

        public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(int userId)
        {
            return await _dapperRepository.QueryAsync<string>(
                ProcedureName,
                new { UserRoleAction = "GetUserPermissionNames", UserId = userId });
        }

        public async Task<User?> GetUserWithRolesAndPermissionsAsync(int userId)
        {
            var result = await _dapperRepository.QueryMultipleAsync(
                ProcedureName,
                new { UserRoleAction = "GetUserWithRolesAndPermissions", UserId = userId });

            try
            {
                var user = await result.ReadFirstOrDefaultAsync<User>();
                if (user == null) return null;

                var roleNames = await result.ReadAsync<string>();
                var permissions = await result.ReadAsync<string>();

                user.Role = roleNames.FirstOrDefault();
                user.Permissions = permissions.ToList();

                return user;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new { UserRoleAction = "HasPermission", UserId = userId, PermissionName = permissionName });

            return result;
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new { UserRoleAction = "IsInRole", UserId = userId, RoleName = roleName });

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName)
        {
            return await _dapperRepository.QueryAsync<User>(
                ProcedureName,
                new { UserRoleAction = "GetUsersInRole", RoleName = roleName });
        }

        public async Task<IEnumerable<User>> GetAllUsersWithRolesAsync()
        {
            return await _dapperRepository.QueryAsync<User>(
                ProcedureName,
                new { UserRoleAction = "GetAllUsersWithRoles" });
        }

        public int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return null;
            return userId;
        }
    }
}
