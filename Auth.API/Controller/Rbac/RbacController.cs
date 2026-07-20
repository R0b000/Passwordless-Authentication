using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shared.Core.Models.Common;
using Shared.Core.Models.Rbac;
using Shared.Core.Models.Entities;
using Shared.Core.Wrapper;
using WrapperResponse = Shared.Core.Wrapper.Response;
using Auth.API.Service.Interface.Rbac;

namespace Auth.API.Controller.Rbac
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RbacController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IUserRoleService _userRoleService;

        public RbacController(IRoleService roleService, IPermissionService permissionService,
            IUserRoleService userRoleService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
            _userRoleService = userRoleService;
        }

        [HttpPost("roles")]
        [Authorize(Policy = "ManageRoles")]
        public async Task<ActionResult<RoleResponse>> CreateRole([FromBody] CreateRoleRequest request)
        {
            var roleResult = await _roleService.CreateRoleAsync(request.Name, request.Description);
            var role = roleResult.Data;
            if (role == null)
            {
                return BadRequest(WrapperResponse.Fail("Role already exists or creation failed"));
            }

            if (request.PermissionNames != null && request.PermissionNames.Any())
            {
                var permissionsResult = await _permissionService.GetPermissionsByNamesAsync(request.PermissionNames);
                foreach (var perm in permissionsResult.Data ?? Enumerable.Empty<Shared.Core.Models.Entities.Permission>())
                {
                    await _roleService.AssignPermissionToRoleAsync(role.Id, perm.Id);
                }
            }

            var roleDtoResult = await _roleService.GetRoleWithPermissionsAsync(role.Id);
            var roleDto = roleDtoResult.Data;
            return Ok(new RoleResponse
            {
                Id = roleDto!.Id,
                Name = roleDto.Name,
                Description = roleDto.Description,
                Permissions = roleDto.Permissions,
                Message = "Role created successfully"
            });
        }

        [HttpGet("roles")]
        public async Task<ActionResult<PaginatedResponse<RoleDto>>> GetAllRoles([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var allRoles = (await _roleService.GetAllRolesAsync()).Data ?? Enumerable.Empty<Shared.Core.Models.Entities.Role>();
            var totalCount = allRoles.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;
            var pagedRoles = allRoles.Skip(skip).Take(pageSize);

            var result = new List<RoleDto>();
            foreach (var role in pagedRoles)
            {
                var roleDto = (await _roleService.GetRoleWithPermissionsAsync(role.Id)).Data;
                if (roleDto != null)
                {
                    result.Add(roleDto);
                }
            }

            return Ok(new PaginatedResponse<RoleDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = result
            });
        }

        [HttpGet("roles/{roleId}")]
        public async Task<ActionResult<RoleDto>> GetRole(int roleId)
        {
            var roleDto = (await _roleService.GetRoleWithPermissionsAsync(roleId)).Data;
            if (roleDto == null)
            {
                return NotFound(WrapperResponse.Fail("Role not found"));
            }

            return Ok(roleDto);
        }

        [HttpPost("roles/permissions")]
        [Authorize(Policy = "ManageRoles")]
        public async Task<ActionResult<IResponse>> AssignPermission([FromBody] AssignPermissionRequest request)
        {
            var assigned = await _roleService.AssignPermissionToRoleAsync(request.RoleId, request.PermissionId);
            if (!assigned.Data)
            {
                return BadRequest(WrapperResponse.Fail("Failed to assign permission"));
            }

            return Ok(WrapperResponse.Success("Permission assigned successfully"));
        }

        [HttpDelete("roles/permissions")]
        [Authorize(Policy = "ManageRoles")]
        public async Task<ActionResult<IResponse>> RemovePermission([FromBody] AssignPermissionRequest request)
        {
            var removed = await _roleService.RemovePermissionFromRoleAsync(request.RoleId, request.PermissionId);
            if (!removed.Data)
            {
                return BadRequest(WrapperResponse.Fail("Failed to remove permission"));
            }

            return Ok(WrapperResponse.Success("Permission removed successfully"));
        }

        [HttpPost("users/roles")]
        [Authorize(Policy = "ManageUsers")]
        public async Task<ActionResult<IResponse>> AssignRoleToUser([FromBody] AssignRoleRequest request)
        {
            var assigned = await _userRoleService.AssignRoleToUserAsync(request.UserId, request.RoleId);
            if (!assigned.Data)
            {
                return BadRequest(WrapperResponse.Fail("Failed to assign role to user"));
            }

            return Ok(WrapperResponse.Success("Role assigned to user successfully"));
        }

        [HttpDelete("users/roles")]
        [Authorize(Policy = "ManageUsers")]
        public async Task<ActionResult<IResponse>> RemoveRoleFromUser([FromBody] AssignRoleRequest request)
        {
            var removed = await _userRoleService.RemoveRoleFromUserAsync(request.UserId, request.RoleId);
            if (!removed.Data)
            {
                return BadRequest(WrapperResponse.Fail("Failed to remove role from user"));
            }

            return Ok(WrapperResponse.Success("Role removed from user successfully"));
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(int userId)
        {
            var roles = await _userRoleService.GetUserRoleNamesAsync(userId);
            return Ok(roles.Data);
        }

        [HttpGet("users/{userId}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(int userId)
        {
            var permissions = await _userRoleService.GetUserPermissionNamesAsync(userId);
            return Ok(permissions.Data);
        }

        [HttpGet("users")]
        public async Task<ActionResult<PaginatedResponse<UserRoleResponse>>> GetAllUsersWithRoles([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var allUsers = (await _userRoleService.GetAllUsersWithRolesAsync()).Data ?? Enumerable.Empty<Shared.Core.Models.Entities.User>();
            var totalCount = allUsers.Count();
            var skip = (page - 1) * pageSize;
            var pagedUsers = allUsers.Skip(skip).Take(pageSize);

            var result = new List<UserRoleResponse>();
            foreach (var user in pagedUsers)
            {
                var userWithData = (await _userRoleService.GetUserWithRolesAndPermissionsAsync(user.Id)).Data;
                if (userWithData != null)
                {
                    result.Add(new UserRoleResponse
                    {
                        UserId = userWithData.Id,
                        Username = userWithData.Username ?? string.Empty,
                        Email = userWithData.Email ?? string.Empty,
                        Role = userWithData.Role,
                        Permissions = userWithData.Permissions
                    });
                }
            }

            return Ok(new PaginatedResponse<UserRoleResponse>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = result
            });
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<PaginatedResponse<PermissionDto>>> GetAllPermissions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var allPermissions = (await _permissionService.GetAllPermissionsAsync()).Data ?? Enumerable.Empty<Shared.Core.Models.Entities.Permission>();
            var totalCount = allPermissions.Count();
            var skip = (page - 1) * pageSize;
            var pagedPermissions = allPermissions.Skip(skip).Take(pageSize);

            var result = pagedPermissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Module = p.Module
            }).ToList();

            return Ok(new PaginatedResponse<PermissionDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = result
            });
        }
    }

    public class RoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
        public string? Message { get; set; }
    }

    public class UserRoleResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public List<string>? Permissions { get; set; }
    }

    public class AssignPermissionRequest
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }
}
