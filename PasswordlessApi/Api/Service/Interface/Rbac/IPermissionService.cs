using PasswordlessApi.Api.Models.DTOs.Rbac;
using PasswordlessApi.Api.Models.Entities;

namespace PasswordlessApi.Api.Service.Interface.Rbac
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
