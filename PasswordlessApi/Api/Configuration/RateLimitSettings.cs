namespace PasswordlessApi.Api.Configuration
{
    /// <summary>
    /// Configuration for the named rate-limiting policies. Bound from the
    /// "RateLimiting" section of appsettings.json so limits can be tuned without
    /// recompiling. Sensible production defaults are provided if the section is absent.
    /// </summary>
    public class RateLimitSettings
    {
        public const string SectionName = "RateLimiting";

        public RateLimitPolicy Login { get; set; } = new();
        public RateLimitPolicy RefreshToken { get; set; } = new();
        public RateLimitPolicy General { get; set; } = new();
        public RateLimitPolicy Registration { get; set; } = new();
    }

    public class RateLimitPolicy
    {
        public int PermitLimit { get; set; } = 100;
        public int WindowMinutes { get; set; } = 1;
    }
}
