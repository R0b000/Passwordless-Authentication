using Microsoft.AspNetCore.Mvc;
using PasswordlessApi.Api.Models.DTOs.Rbac;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Rbac;

namespace PasswordlessApi.Api.Controller.Rbac
{
    [Route("api/[controller]")]
    [ApiController]
    public class RbacController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IUserRoleService _userRoleService;
        private readonly IAuthService _authService;

        public RbacController(IRoleService roleService, IPermissionService permissionService,
            IUserRoleService userRoleService, IAuthService authService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
            _userRoleService = userRoleService;
            _authService = authService;
        }

        [HttpPost("roles")]
        public async Task<ActionResult<AuthResponse>> CreateRole([FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.CreateRoleAsync(request.Name, request.Description);
            if (role == null)
            {
                return BadRequest(new AuthResponse { Message = "Role already exists or creation failed" });
            }

            if (request.PermissionNames != null && request.PermissionNames.Any())
            {
                var permissions = await _permissionService.GetPermissionsByNamesAsync(request.PermissionNames);
                foreach (var perm in permissions)
                {
                    await _roleService.AssignPermissionToRoleAsync(role.Id, perm.Id);
                }
            }

            var roleDto = await _roleService.GetRoleWithPermissionsAsync(role.Id);
            return Ok(new AuthResponse
            {
                UserId = role.Id,
                Username = role.Name,
                Message = "Role created successfully"
            });
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var result = new List<object>();

            foreach (var role in roles)
            {
                var roleDto = await _roleService.GetRoleWithPermissionsAsync(role.Id);
                if (roleDto != null)
                {
                    result.Add(roleDto);
                }
            }

            return Ok(result);
        }

        [HttpGet("roles/{roleId}")]
        public async Task<ActionResult<RoleDto>> GetRole(int roleId)
        {
            var roleDto = await _roleService.GetRoleWithPermissionsAsync(roleId);
            if (roleDto == null)
            {
                return NotFound(new AuthResponse { Message = "Role not found" });
            }

            return Ok(roleDto);
        }

        [HttpPost("roles/permissions")]
        public async Task<ActionResult<AuthResponse>> AssignPermission([FromBody] AssignRoleRequest request)
        {
            var assigned = await _roleService.AssignPermissionToRoleAsync(request.RoleId, request.PermissionId);
            if (!assigned)
            {
                return BadRequest(new AuthResponse { Message = "Failed to assign permission" });
            }

            return Ok(new AuthResponse { Message = "Permission assigned successfully" });
        }

        [HttpDelete("roles/permissions")]
        public async Task<ActionResult<AuthResponse>> RemovePermission([FromBody] AssignRoleRequest request)
        {
            var removed = await _roleService.RemovePermissionFromRoleAsync(request.RoleId, request.PermissionId);
            if (!removed)
            {
                return BadRequest(new AuthResponse { Message = "Failed to remove permission" });
            }

            return Ok(new AuthResponse { Message = "Permission removed successfully" });
        }

        [HttpPost("users/roles")]
        public async Task<ActionResult<AuthResponse>> AssignRoleToUser([FromBody] AssignRoleRequest request)
        {
            var assigned = await _userRoleService.AssignRoleToUserAsync(request.UserId, request.RoleId);
            if (!assigned)
            {
                return BadRequest(new AuthResponse { Message = "Failed to assign role to user" });
            }

            return Ok(new AuthResponse { Message = "Role assigned to user successfully" });
        }

        [HttpDelete("users/roles")]
        public async Task<ActionResult<AuthResponse>> RemoveRoleFromUser([FromBody] AssignRoleRequest request)
        {
            var removed = await _userRoleService.RemoveRoleFromUserAsync(request.UserId, request.RoleId);
            if (!removed)
            {
                return BadRequest(new AuthResponse { Message = "Failed to remove role from user" });
            }

            return Ok(new AuthResponse { Message = "Role removed from user successfully" });
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(int userId)
        {
            var roles = await _userRoleService.GetUserRoleNamesAsync(userId);
            return Ok(roles);
        }

        [HttpGet("users/{userId}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(int userId)
        {
            var permissions = await _userRoleService.GetUserPermissionNamesAsync(userId);
            return Ok(permissions);
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllUsersWithRoles()
        {
            var users = await _userRoleService.GetAllUsersWithRolesAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                var userWithData = await _userRoleService.GetUserWithRolesAndPermissionsAsync(user.Id);
                if (userWithData != null)
                {
                    result.Add(new
                    {
                        UserId = userWithData.Id,
                        Username = userWithData.Username,
                        Email = userWithData.Email,
                        Role = userWithData.Role,
                        Permissions = userWithData.Permissions
                    });
                }
            }

            return Ok(result);
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            var result = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Module = p.Module
            }).ToList();

            return Ok(result);
        }
    }
}
