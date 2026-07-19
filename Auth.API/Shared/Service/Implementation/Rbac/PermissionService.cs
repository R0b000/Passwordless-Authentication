using API.Shared.Common;
using Shared.Core.Models.Entities;
using API.Shared.Service.Interface.Rbac;
using Shared.Data.Repository.Interface;
using Shared.Core.Wrapper;

namespace API.Shared.Service.Implementation.Rbac
{
    public class PermissionService : IPermissionService
    {
        private readonly IDapperRepository _dapperRepository;

        public PermissionService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<IResponse<Permission?>> CreatePermissionAsync(string name, string? description, string? module)
        {
            var id = (await _dapperRepository.ExecuteAsync(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.CreatePermission, Name = name, Description = description, Module = module }));

            if (id <= 0) return Response<Permission?>.Fail("Failed to create permission");

            return Response<Permission?>.Success((await GetPermissionByNameAsync(name)).Data);
        }

        public async Task<IResponse<IEnumerable<Permission>>> GetAllPermissionsAsync()
        {
            var result = (await _dapperRepository.QueryAsync<Permission>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetAllPermissions }));

            return Response<IEnumerable<Permission>>.Success(result ?? Enumerable.Empty<Permission>());
        }

        public async Task<IResponse<IEnumerable<Permission>>> GetPermissionsByNamesAsync(IEnumerable<string> names)
        {
            if (!names.Any()) return Response<IEnumerable<Permission>>.Success(Enumerable.Empty<Permission>());

            var result = (await _dapperRepository.QueryAsync<Permission>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetPermissionsByNames, Names = string.Join(",", names) }));

            return Response<IEnumerable<Permission>>.Success(result ?? Enumerable.Empty<Permission>());
        }

        public async Task<IResponse<Permission?>> GetPermissionByNameAsync(string name)
        {
            var result = (await _dapperRepository.QueryFirstAsync<Permission>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetPermissionByName, Name = name }));

            if (result == null)
                return Response<Permission?>.Fail("Permission not found");

            return Response<Permission?>.Success(result);
        }

        public async Task<IResponse<IEnumerable<string>>> GetPermissionNamesByRoleIdAsync(int roleId)
        {
            var result = (await _dapperRepository.QueryAsync<string>(
                DbConstants.Procedures.Rbac,
                new { PermissionAction = DbConstants.RbacActions.GetPermissionNamesByRoleId, RoleId = roleId }));

            return Response<IEnumerable<string>>.Success(result ?? Enumerable.Empty<string>());
        }

        public async Task<IResponse> SeedDefaultPermissionsAsync()
        {
            var existing = (await GetAllPermissionsAsync()).Data ?? Enumerable.Empty<Permission>();
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

            return Response.Success("Default permissions seeded");
        }
    }
}
