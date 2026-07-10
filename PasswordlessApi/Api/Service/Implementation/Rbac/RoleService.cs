using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.DTOs.Rbac;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Rbac;

namespace PasswordlessApi.Api.Service.Implementation.Rbac
{
    public class RoleService : IRoleService
    {
        private readonly IDapperRepository _dapperRepository;
        private const string ProcedureName = "sp_RBAC";

        public RoleService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<Role?> CreateRoleAsync(string name, string? description)
        {
            var id = await _dapperRepository.ExecuteAsync(
                ProcedureName,
                new { RoleAction = "CreateRole", Name = name, Description = description });

            if (id <= 0) return null;

            return await GetRoleByNameAsync(name);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _dapperRepository.QueryAsync<Role>(
                ProcedureName,
                new { RoleAction = "GetAllRoles" });
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            return await _dapperRepository.QueryFirstAsync<Role>(
                ProcedureName,
                new { RoleAction = "GetRoleByName", Name = name });
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _dapperRepository.QueryFirstAsync<Role>(
                ProcedureName,
                new { RoleAction = "GetRoleById", RoleId = roleId });
        }

        public async Task<RoleDto?> GetRoleWithPermissionsAsync(int roleId)
        {
            var result = await _dapperRepository.QueryMultipleAsync(
                ProcedureName,
                new { RoleAction = "GetRoleWithPermissions", RoleId = roleId });

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
                ProcedureName,
                new { RoleAction = "AssignPermissionToRole", RoleId = roleId, PermissionId = permissionId });

            return result;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new { RoleAction = "RemovePermissionFromRole", RoleId = roleId, PermissionId = permissionId });

            return result;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var result = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new { RoleAction = "DeleteRole", RoleId = roleId });

            return result;
        }

        public async Task<bool> IsSystemRoleAsync(int roleId)
        {
            var role = await GetRoleByIdAsync(roleId);
            return role?.IsSystemRole ?? false;
        }
    }
}
