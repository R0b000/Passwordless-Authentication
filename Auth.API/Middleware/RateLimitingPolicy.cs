using Auth.API.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Auth.API.Utility.Http;

namespace Auth.API.Middleware
{
    public static class SecurityRateLimiting
    {
        public const string LoginPolicy = "login";
        public const string RefreshTokenPolicy = "refresh-token";
        public const string GeneralPolicy = "general";
        public const string RegistrationPolicy = "registration";

        public static void AddSecurityRateLimiting(this IServiceCollection services, RateLimitSettings settings)
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy(LoginPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.Login.PermitLimit,
                        Window = TimeSpan.FromMinutes(settings.Login.WindowMinutes),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.AddPolicy(RefreshTokenPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.RefreshToken.PermitLimit,
                        Window = TimeSpan.FromMinutes(settings.RefreshToken.WindowMinutes),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.AddPolicy(GeneralPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.General.PermitLimit,
                        Window = TimeSpan.FromMinutes(settings.General.WindowMinutes),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

                options.AddPolicy(RegistrationPolicy, context => CreateFixedWindowLimiter(
                    context,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.Registration.PermitLimit,
                        Window = TimeSpan.FromMinutes(settings.Registration.WindowMinutes),
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
            var ip = context.GetClientIpAddress() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(ip, factory);
        }
    }
}
