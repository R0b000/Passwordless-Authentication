using PasswordlessApi.Api.Common;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Rbac;

namespace PasswordlessApi.Api.Service.Implementation.Rbac
{
    public class PermissionService : IPermissionService
    {
        private readonly IDapperRepository _dapperRepository;

        public PermissionService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<Permission?> CreatePermissionAsync(string name, string? description, string? module)
        {
            var id = await _dapperRepository.ExecuteAsync(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.CreatePermission, Name = name, Description = description, Module = module });

            if (id <= 0) return null;

            return await GetPermissionByNameAsync(name);
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _dapperRepository.QueryAsync<Permission>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetAllPermissions });
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByNamesAsync(IEnumerable<string> names)
        {
            if (!names.Any()) return Enumerable.Empty<Permission>();

            return await _dapperRepository.QueryAsync<Permission>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetPermissionsByNames, Names = string.Join(",", names) });
        }

        public async Task<Permission?> GetPermissionByNameAsync(string name)
        {
            return await _dapperRepository.QueryFirstAsync<Permission>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetPermissionByName, Name = name });
        }

        public async Task<IEnumerable<string>> GetPermissionNamesByRoleIdAsync(int roleId)
        {
            return await _dapperRepository.QueryAsync<string>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetPermissionNamesByRoleId, RoleId = roleId });
        }

        public async Task SeedDefaultPermissionsAsync()
        {
            var existing = await GetAllPermissionsAsync();
            var existingNames = new HashSet<string>(existing.Select(p => p.Name.ToLowerInvariant()));

            var defaultPermissions = new[]
            {
                new { Name = "users.read", Description = "View users", Module = "Users" },
                new { Name = "users.write", Description = "Create/update users", Module = "Users" },
                new { Name = "users.delete", Description = "Delete users", Module = "Users" },
                new { Name = "roles.read", Description = "View roles", Module = "Roles" },
                new { Name = "roles.write", Description = "Create/update roles", Module = "Roles" },
                new { Name = "roles.delete", Description = "Delete roles", Module = "Roles" },
                new { Name = "auth.read", Description = "View auth settings", Module = "Auth" },
                new { Name = "auth.write", Description = "Modify auth settings", Module = "Auth" },
                new { Name = "dashboard.view", Description = "Access dashboard", Module = "Dashboard" }
            };

            foreach (var perm in defaultPermissions)
            {
                if (!existingNames.Contains(perm.Name.ToLowerInvariant()))
                {
                    await CreatePermissionAsync(perm.Name, perm.Description, perm.Module);
                }
            }
        }
    }
}
