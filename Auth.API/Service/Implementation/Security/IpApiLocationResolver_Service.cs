using System.Net;
using Auth.API.Service.Interface.Security;

namespace Auth.API.Service.Implementation.Security
{
    public class IpApiLocationResolver : ILocationResolver
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IpApiLocationResolver> _logger;

        public IpApiLocationResolver(HttpClient httpClient, ILogger<IpApiLocationResolver> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string?> ResolveLocationAsync(string? ipAddress, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "127.0.0.1" || ipAddress == "::1")
            {
                return "Localhost";
            }

            try
            {
                using var response = await _httpClient.GetAsync($"https://ipapi.co/{ipAddress}/json/", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("error", out _))
                {
                    return null;
                }

                var city = doc.RootElement.TryGetProperty("city", out var c) ? c.GetString() : null;
                var country = doc.RootElement.TryGetProperty("country_name", out var co) ? co.GetString() : null;
                return $"{city}, {country}".Trim(',', ' ');
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve location for IP {IpAddress}", ipAddress);
            }

            return null;
        }
    }
}