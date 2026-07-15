using API.Shared.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Shared.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly ApiSettings _apiSettings;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger, IOptions<ApiSettings> apiSettings)
        {
            _next = next;
            _logger = logger;
            _apiSettings = apiSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            context.Response.Headers["Content-Security-Policy"] = BuildContentSecurityPolicy();
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
            context.Response.Headers.Remove("Server");

            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            await _next(context);
        }

        private string BuildContentSecurityPolicy()
        {
            var origins = _apiSettings.GetAllowedOrigins();
            var cspSources = origins.Concat(_apiSettings.CspExtraSources ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var originDirectives = cspSources.Length > 0 ? string.Join(" ", cspSources) : "'self'";

            // IMPORTANT: Swagger UI requires 'unsafe-inline' and 'unsafe-eval' to function!
            return $@"
                default-src 'self';
                script-src 'self' 'unsafe-inline' 'unsafe-eval' {originDirectives};
                style-src 'self' 'unsafe-inline';
                img-src 'self' data: https:;
                font-src 'self' data:;
                connect-src 'self' {originDirectives};
                frame-ancestors 'none';
            ";
        }
    }
}
