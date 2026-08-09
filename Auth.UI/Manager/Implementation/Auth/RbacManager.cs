using Microsoft.AspNetCore.Components;
using Shared.Data.Wrapper;
using Auth.Model.Token;
using Auth.UI.Manager.Interface.Auth;
using Shared.UI.Http;
using Auth.UI.Manager.Routes;
using Auth.Model.Models.Rbac;
using Auth.Model.Models.Common;
using Auth.Model.Models.Security;

namespace Auth.UI.Manager.Implementation.Auth
{
    public class RbacManager : IRbacManager
    {
        private readonly IHttpServices _httpService;
        private readonly ITokenStore _tokenStore;

        public RbacManager(IHttpServices httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
        }

        public async Task<IResponse<PaginatedResponse<UserRoleResponse>>> GetUsersWithRolesAsync(int page, int pageSize)
        {
            try
            {
                var url = $"{RbacRoute.GetAllUsers}?page={page}&pageSize={pageSize}";
                var result = await _httpService.GetAsync<PaginatedResponse<UserRoleResponse>>(url);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<PaginatedResponse<UserRoleResponse>>.Success(result.Data, "Users retrieved successfully");
                }
                else
                {
                    return Response<PaginatedResponse<UserRoleResponse>>.Fail(result.Messages ?? "Failed to retrieve users");
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Response<PaginatedResponse<UserRoleResponse>>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResponse<PaginatedResponse<RoleDto>>> GetRolesAsync(int page, int pageSize)
        {
            try
            {
                var url = $"{RbacRoute.GetAllRoles}?page={page}&pageSize={pageSize}";
                var result = await _httpService.GetAsync<PaginatedResponse<RoleDto>>(url);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<PaginatedResponse<RoleDto>>.Success(result.Data, "Roles retrieved successfully");
                }
                else
                {
                    return Response<PaginatedResponse<RoleDto>>.Fail(result.Messages ?? "Failed to retrieve roles");
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Response<PaginatedResponse<RoleDto>>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResponse<PaginatedResponse<PermissionDto>>> GetPermissionsAsync(int page, int pageSize)
        {
            try
            {
                var url = $"{RbacRoute.GetAllPermissions}?page={page}&pageSize={pageSize}";
                var result = await _httpService.GetAsync<PaginatedResponse<PermissionDto>>(url);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<PaginatedResponse<PermissionDto>>.Success(result.Data, "Permissions retrieved successfully");
                }
                else
                {
                    return Response<PaginatedResponse<PermissionDto>>.Fail(result.Messages ?? "Failed to retrieve permissions");
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Response<PaginatedResponse<PermissionDto>>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> AssignRoleToUserAsync(int userId, int roleId)
        {
            try
            {
                var request = new AssignRoleRequest { UserId = userId, RoleId = roleId };
                var result = await _httpService.PostAsJsonAsync<ActionResponse>(RbacRoute.AssignRole, request);
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Role assigned successfully");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to assign role");
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResponse<bool>> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var request = new AssignRoleRequest { UserId = userId, RoleId = roleId };
                var result = await _httpService.DeleteAsJsonAsync<ActionResponse>(RbacRoute.RemoveRole, request);
                if (result.Succeeded)
                {
                    return Response<bool>.Success(true, result.Messages ?? "Role removed successfully");
                }
                else
                {
                    return Response<bool>.Fail(result.Messages ?? "Failed to remove role");
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResponse<IEnumerable<string>>> GetUserRolesAsync(int userId)
        {
            try
            {
                var url = $"/api/Rbac/users/{userId}/roles";
                var result = await _httpService.GetAsync<IEnumerable<string>>(url);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<IEnumerable<string>>.Success(result.Data, "User roles retrieved successfully");
                }
                else
                {
                    return Response<IEnumerable<string>>.Fail(result.Messages ?? "Failed to retrieve user roles");
                }
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<string>>.Fail($"An error occurred: {ex.Message}");
            }
        }
    }
}
