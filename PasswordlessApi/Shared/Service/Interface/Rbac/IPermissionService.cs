using API.Shared.Models.Entities;

namespace API.Shared.Service.Interface.Rbac
{
    public interface IPermissionService
    {
        Task<Permission?> CreatePermissionAsync(string name, string? description, string? module);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<IEnumerable<Permission>> GetPermissionsByNamesAsync(IEnumerable<string> names);
        Task<Permission?> GetPermissionByNameAsync(string name);
        Task<IEnumerable<string>> GetPermissionNamesByRoleIdAsync(int roleId);
        Task SeedDefaultPermissionsAsync();
    }
}
