using API.Shared.Common;
using Shared.Core.Models.Entities;
using Shared.Core.Models.DTOs.Rbac;
using API.Shared.Service.Interface.Rbac;
using Shared.Data.Repository.Interface;
using Shared.Core.Wrapper;

namespace API.Shared.Service.Implementation.Rbac
{
    public class RoleService : IRoleService
    {
        private readonly IDapperRepository _dapperRepository;

        public RoleService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<IResponse<Role?>> CreateRoleAsync(string name, string? description)
        {
            var id = (await _dapperRepository.ExecuteAsync(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.CreateRole, Name = name, Description = description }));

            if (id <= 0) return Response<Role?>.Fail("Failed to create role");

            return Response<Role?>.Success((await GetRoleByNameAsync(name)).Data);
        }

        public async Task<IResponse<IEnumerable<Role>>> GetAllRolesAsync()
        {
            var result = (await _dapperRepository.QueryAsync<Role>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetAllRoles }));

            return Response<IEnumerable<Role>>.Success(result ?? Enumerable.Empty<Role>());
        }

        public async Task<IResponse<Role?>> GetRoleByNameAsync(string name)
        {
            var result = (await _dapperRepository.QueryFirstAsync<Role>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetRoleByName, Name = name }));

            if (result == null)
                return Response<Role?>.Fail("Role not found");

            return Response<Role?>.Success(result);
        }

        public async Task<IResponse<Role?>> GetRoleByIdAsync(int roleId)
        {
            var result = (await _dapperRepository.QueryFirstAsync<Role>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetRoleById, RoleId = roleId }));

            if (result == null)
                return Response<Role?>.Fail("Role not found");

            return Response<Role?>.Success(result);
        }

        public async Task<IResponse<RoleDto?>> GetRoleWithPermissionsAsync(int roleId)
        {
            using var result = (await _dapperRepository.QueryMultipleAsync(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetRoleWithPermissions, RoleId = roleId }))!;

            try
            {
                var role = await result.ReadFirstOrDefaultAsync<Role>();
                if (role == null) return Response<RoleDto?>.Fail("Role not found");

                var permissions = await result.ReadAsync<Permission>();
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsSystemRole = role.IsSystemRole,
                    Permissions = permissions.Select(p => p.Name).ToList()
                };

                return Response<RoleDto?>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return Response<RoleDto?>.Fail(ex.Message);
            }
        }

        public async Task<IResponse<bool>> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.AssignPermissionToRole, RoleId = roleId, PermissionId = permissionId }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<bool>> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.RemovePermissionFromRole, RoleId = roleId, PermissionId = permissionId }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<bool>> DeleteRoleAsync(int roleId)
        {
            var result = (await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.DeleteRole, RoleId = roleId }));

            return Response<bool>.Success(result);
        }

        public async Task<IResponse<bool>> IsSystemRoleAsync(int roleId)
        {
            var role = (await GetRoleByIdAsync(roleId)).Data;
            return Response<bool>.Success(role?.IsSystemRole ?? false);
        }
    }
}
