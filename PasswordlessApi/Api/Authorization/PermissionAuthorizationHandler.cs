using Microsoft.AspNetCore.Authorization;
using PasswordlessApi.Api.Service.Interface.Rbac;
using PasswordlessApi.Api.Common;

namespace PasswordlessApi.Api.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(IUserRoleService userRoleService, IHttpContextAccessor httpContextAccessor)
        {
            _userRoleService = userRoleService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.GetUserId();
            if (userId == null)
            {
                context.Fail();
                return;
            }

            var hasPermission = await _userRoleService.HasPermissionAsync(userId.Value, requirement.Permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
