using API.Shared.Common;
using API.Shared.Service.Interface.Rbac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.Shared.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
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

            var httpContext = _httpContextAccessor.HttpContext ?? context.Resource as HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var userRoleService = httpContext.RequestServices.GetRequiredService<IUserRoleService>();
            var hasPermission = await userRoleService.HasPermissionAsync(userId.Value, requirement.Permission);
            if (hasPermission.Data)
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
