namespace PasswordlessApi.Api.Configuration
{
    /// <summary>
    /// Centralized, strongly typed configuration for API-related settings.
    /// This is the single source of truth for the public API base URL and the
    /// authorized front-end URLs (origins) that are shared by both the CORS
    /// policy and the FIDO2 relying-party configuration.
    /// </summary>
    public class ApiSettings
    {
        public const string SectionName = "ApiSettings";

        /// <summary>
        /// The public base URL of this API (e.g. used by clients / documentation).
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:5000";

        /// <summary>
        /// Friendly relying-party name shown to authenticators during FIDO2 ceremonies.
        /// </summary>
        public string ServerName { get; set; } = "PasswordlessApi";

        /// <summary>
        /// Optional explicit FIDO2 relying-party id (domain/host). When left empty it is
        /// resolved dynamically from the request origin, otherwise from the first
        /// authorized origin. See <see cref="ResolveServerDomain"/>.
        /// </summary>
        public string? ServerDomain { get; set; }

        /// <summary>
        /// Whether CORS responses should allow credentials (cookies / auth headers).
        /// </summary>
        public bool AllowCredentials { get; set; } = true;

        /// <summary>
        /// All authorized front-end URLs. Used both for the CORS policy and for
        /// FIDO2 origin validation. Mapped from appsettings.json as an array.
        /// </summary>
        public List<string> AllowedOrigins { get; set; } = new();

        /// <summary>
        /// Returns the configured origins as a clean array (trimmed, de-duplicated,
        /// non-empty). Safe to hand directly to <c>policy.WithOrigins(...)</c>.
        /// </summary>
        public string[] GetAllowedOrigins() =>
            AllowedOrigins
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        /// <summary>
        /// True when the configuration represents an "allow any origin" wildcard.
        /// </summary>
        public bool IsWildcardOrigin()
        {
            var origins = GetAllowedOrigins();
            return origins.Length == 1 && origins[0] == "*";
        }

        /// <summary>
        /// Resolves the default FIDO2 relying-party id (domain/host) used when a
        /// ceremony does not carry an explicit origin. Preference order:
        /// explicit <see cref="ServerDomain"/> -> host of the first authorized
        /// origin -> host of <see cref="BaseUrl"/> -> "localhost".
        /// </summary>
        public string ResolveServerDomain()
        {
            if (!string.IsNullOrWhiteSpace(ServerDomain))
                return ServerDomain.Trim();

            foreach (var origin in GetAllowedOrigins())
            {
                if (origin != "*" && TryGetHost(origin, out var host))
                    return host;
            }

            if (TryGetHost(BaseUrl, out var baseHost))
                return baseHost;

            return "localhost";
        }

        private static bool TryGetHost(string value, out string host)
        {
            host = string.Empty;
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                host = uri.Host;
                return true;
            }

            return false;
        }
    }
}
