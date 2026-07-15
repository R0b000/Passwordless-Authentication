using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using API.Shared.Common;
using API.Shared.Models.Entities;
using API.Shared.Service.Interface.Rbac;
using API.Shared.Service.Interface.Repository;

namespace API.Shared.Service.Implementation.Rbac
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRoleService(IDapperRepository dapperRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dapperRepository = dapperRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.AssignRoleToUser, UserId = userId, RoleId = roleId })).Data;

            return result;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.RemoveRoleFromUser, UserId = userId, RoleId = roleId })).Data;

            return result;
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId)
        {
            return (await _dapperRepository.QueryAsync<UserRole>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserRoles, UserId = userId })).Data ?? Enumerable.Empty<UserRole>();
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
        {
            return (await _dapperRepository.QueryAsync<string>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserRoleNames, UserId = userId })).Data ?? Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(int userId)
        {
            return (await _dapperRepository.QueryAsync<string>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserPermissionNames, UserId = userId })).Data ?? Enumerable.Empty<string>();
        }

        public async Task<User?> GetUserWithRolesAndPermissionsAsync(int userId)
        {
            using var result = (await _dapperRepository.QueryMultipleAsync(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserWithRolesAndPermissions, UserId = userId })).Data!;

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
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.HasPermission, UserId = userId, PermissionName = permissionName })).Data;

            return result;
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.IsInRole, UserId = userId, RoleName = roleName })).Data;

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName)
        {
            return (await _dapperRepository.QueryAsync<User>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUsersInRole, RoleName = roleName })).Data ?? Enumerable.Empty<User>();
        }

        public async Task<IEnumerable<User>> GetAllUsersWithRolesAsync()
        {
            return (await _dapperRepository.QueryAsync<User>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetAllUsersWithRoles })).Data ?? Enumerable.Empty<User>();
        }

        public int? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }
    }
}
