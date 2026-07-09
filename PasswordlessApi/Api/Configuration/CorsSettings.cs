namespace PasswordlessApi.Api.Configuration
{
    public class CorsSettings
    {
        public string AllowedOrigins { get; set; } = string.Empty;
        public string AllowedMethods { get; set; } = "GET,POST,PUT,DELETE,OPTIONS";
        public string AllowedHeaders { get; set; } = "Content-Type,Authorization,X-Requested-With";
        public bool AllowCredentials { get; set; } = true;
    }
}