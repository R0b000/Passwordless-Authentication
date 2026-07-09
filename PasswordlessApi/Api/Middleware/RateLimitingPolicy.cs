using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace PasswordlessApi.Api.Middleware
{
    public static class SecurityRateLimiting
    {
        public const string LoginPolicy = "login";
        public const string RefreshTokenPolicy = "refresh-token";
        public const string GeneralPolicy = "general";

        public static void AddSecurityRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(LoginPolicy, windowOptions =>
                {
                    windowOptions.PermitLimit = 5;
                    windowOptions.Window = TimeSpan.FromMinutes(5);
                    windowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    windowOptions.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter(RefreshTokenPolicy, windowOptions =>
                {
                    windowOptions.PermitLimit = 30;
                    windowOptions.Window = TimeSpan.FromMinutes(1);
                    windowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    windowOptions.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter(GeneralPolicy, windowOptions =>
                {
                    windowOptions.PermitLimit = 100;
                    windowOptions.Window = TimeSpan.FromMinutes(1);
                    windowOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    windowOptions.QueueLimit = 0;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, _) =>
                {
                    context.HttpContext.Response.Headers["Retry-After"] = "60";
                    await Task.CompletedTask;
                };
            });
        }
    }
}
