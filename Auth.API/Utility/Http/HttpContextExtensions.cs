using Microsoft.AspNetCore.Http;

namespace Auth.API.Utility.Http;

public static class HttpContextExtensions
{
    public static string? GetClientIpAddress(this HttpContext? context)
    {
        if (context == null) return null;

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

    public static string? GetUserAgent(this HttpContext? context)
    {
        if (context == null) return null;
        return context.Request.Headers["User-Agent"].ToString();
    }
}