using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using API.Shared.Common;
using API.Shared.Models.Entities;
using API.Shared.Service.Interface.Rbac;
using API.Shared.Service.Interface.Repository;
using Shared.Wrapper;

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

        public async Task<IResponse<bool>> AssignRoleToUserAsync(int userId, int roleId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.AssignRoleToUser, UserId = userId, RoleId = roleId }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<bool>> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.RemoveRoleFromUser, UserId = userId, RoleId = roleId }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<IEnumerable<UserRole>>> GetUserRolesAsync(int userId)
        {
            var result = (await _dapperRepository.QueryAsync<UserRole>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserRoles, UserId = userId }));

            return Response<IEnumerable<UserRole>>.Success(result ?? Enumerable.Empty<UserRole>());
        }

        public async Task<IResponse<IEnumerable<string>>> GetUserRoleNamesAsync(int userId)
        {
            var result = (await _dapperRepository.QueryAsync<string>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserRoleNames, UserId = userId }));

            return Response<IEnumerable<string>>.Success(result ?? Enumerable.Empty<string>());
        }

        public async Task<IResponse<IEnumerable<string>>> GetUserPermissionNamesAsync(int userId)
        {
            var result = (await _dapperRepository.QueryAsync<string>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserPermissionNames, UserId = userId }));

            return Response<IEnumerable<string>>.Success(result ?? Enumerable.Empty<string>());
        }

        public async Task<IResponse<User?>> GetUserWithRolesAndPermissionsAsync(int userId)
        {
            using var result = (await _dapperRepository.QueryMultipleAsync(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUserWithRolesAndPermissions, UserId = userId }))!;

            try
            {
                var user = await result.ReadFirstOrDefaultAsync<User>();
                if (user == null) return Response<User?>.Fail("User not found");

                var roleNames = await result.ReadAsync<string>();
                var permissions = await result.ReadAsync<string>();

                user.Role = roleNames.FirstOrDefault();
                user.Permissions = permissions.ToList();

                return Response<User?>.Success(user);
            }
            catch (Exception ex)
            {
                return Response<User?>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<bool>> HasPermissionAsync(int userId, string permissionName)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.HasPermission, UserId = userId, PermissionName = permissionName }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<bool>> IsInRoleAsync(int userId, string roleName)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.IsInRole, UserId = userId, RoleName = roleName }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<IEnumerable<User>>> GetUsersInRoleAsync(string roleName)
        {
            var result = (await _dapperRepository.QueryAsync<User>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetUsersInRole, RoleName = roleName }));

            return Response<IEnumerable<User>>.Success(result ?? Enumerable.Empty<User>());
        }

        public async Task<IResponse<IEnumerable<User>>> GetAllUsersWithRolesAsync()
        {
            var result = (await _dapperRepository.QueryAsync<User>(
                DbConstants.Procedures.Rbac,
                new { UserRoleAction = DbConstants.RbacActions.GetAllUsersWithRoles }));

            return Response<IEnumerable<User>>.Success(result ?? Enumerable.Empty<User>());
        }

        public int? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }
    }
}
