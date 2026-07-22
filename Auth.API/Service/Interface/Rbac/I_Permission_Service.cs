using Auth.Model.Models.Entities;
using Auth.Model.Wrapper;

namespace Auth.API.Service.Interface.Rbac
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

