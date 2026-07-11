using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace PasswordlessApi.Api.Middleware
{
    public static class SecurityRateLimiting
    {
        public const string LoginPolicy = "login";
        public const string RefreshTokenPolicy = "refresh-token";
        public const string GeneralPolicy = "general";
        public const string RegistrationPolicy = "registration";

        public static void AddSecurityRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy(LoginPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.AddPolicy(RefreshTokenPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.AddPolicy(GeneralPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.AddPolicy(RegistrationPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(15),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, _) =>
                {
                    context.HttpContext.Response.Headers["Retry-After"] = "60";
                    await Task.CompletedTask;
                };
            });
        }

        // Partition each policy by client IP so that one client exhausting the
        // limit does not lock out the entire application. Without an explicit
        // partition key the named FixedWindowLimiter policies share a single
        // global bucket across all callers.
        private static RateLimitPartition<string> CreateFixedWindowLimiter(
            HttpContext context,
            Func<string, FixedWindowRateLimiterOptions> factory)
        {
            var ip = GetClientIpAddress(context) ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(ip, factory);
        }

        private static string? GetClientIpAddress(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var first = forwardedFor.ToString().Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(first))
                {
                    return first;
                }
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
