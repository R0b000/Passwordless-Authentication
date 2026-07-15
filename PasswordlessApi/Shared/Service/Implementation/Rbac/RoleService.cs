using API.Shared.Common;
using API.Shared.Models.Entities;
using API.Shared.Models.DTOs.Rbac;
using API.Shared.Service.Interface.Rbac;
using API.Shared.Service.Interface.Repository;

namespace API.Shared.Service.Implementation.Rbac
{
    public class RoleService : IRoleService
    {
        private readonly IDapperRepository _dapperRepository;

        public RoleService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<Role?> CreateRoleAsync(string name, string? description)
        {
            var id = await _dapperRepository.ExecuteAsync(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.CreateRole, Name = name, Description = description });

            if (id <= 0) return null;

            return await GetRoleByNameAsync(name);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _dapperRepository.QueryAsync<Role>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetAllRoles });
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            return await _dapperRepository.QueryFirstAsync<Role>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetRoleByName, Name = name });
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _dapperRepository.QueryFirstAsync<Role>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetRoleById, RoleId = roleId });
        }

        public async Task<RoleDto?> GetRoleWithPermissionsAsync(int roleId)
        {
            using var result = await _dapperRepository.QueryMultipleAsync(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.GetRoleWithPermissions, RoleId = roleId });

            try
            {
                var role = await result.ReadFirstOrDefaultAsync<Role>();
                if (role == null) return null;

                var permissions = await result.ReadAsync<Permission>();
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsSystemRole = role.IsSystemRole,
                    Permissions = permissions.Select(p => p.Name).ToList()
                };

                return roleDto;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.AssignPermissionToRole, RoleId = roleId, PermissionId = permissionId });

            return result;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.RemovePermissionFromRole, RoleId = roleId, PermissionId = permissionId });

            return result;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                DbConstants.Procedures.Rbac,
                new { RoleAction = DbConstants.RbacActions.DeleteRole, RoleId = roleId });

            return result;
        }

        public async Task<bool> IsSystemRoleAsync(int roleId)
        {
            var role = await GetRoleByIdAsync(roleId);
            return role?.IsSystemRole ?? false;
        }
    }
}
