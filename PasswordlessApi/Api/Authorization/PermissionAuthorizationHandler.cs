using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PasswordlessApi.Api.Common;
using PasswordlessApi.Api.Service.Interface.Rbac;

namespace PasswordlessApi.Api.Authorization
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

            // Authorization handlers are singletons, so scoped services (e.g. IUserRoleService
            // and its DapperContext) must be resolved from the request scope, not injected.
            var httpContext = _httpContextAccessor.HttpContext ?? context.Resource as HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var userRoleService = httpContext.RequestServices.GetRequiredService<IUserRoleService>();
            var hasPermission = await userRoleService.HasPermissionAsync(userId.Value, requirement.Permission);
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
