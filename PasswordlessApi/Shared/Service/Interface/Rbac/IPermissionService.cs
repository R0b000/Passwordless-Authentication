using API.Shared.Models.Entities;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Rbac
{
    public interface IPermissionService
    {
        Task<IResponse<Permission?>> CreatePermissionAsync(string name, string? description, string? module);
        Task<IResponse<IEnumerable<Permission>>> GetAllPermissionsAsync();
        Task<IResponse<IEnumerable<Permission>>> GetPermissionsByNamesAsync(IEnumerable<string> names);
        Task<IResponse<Permission?>> GetPermissionByNameAsync(string name);
        Task<IResponse<IEnumerable<string>>> GetPermissionNamesByRoleIdAsync(int roleId);
        Task<IResponse> SeedDefaultPermissionsAsync();
    }
}
