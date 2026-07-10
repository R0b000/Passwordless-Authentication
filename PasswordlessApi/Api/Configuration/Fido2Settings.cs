namespace PasswordlessApi.Api.Configuration
{
    public class Fido2Settings
    {
        public string ServerDomain { get; set; } = "localhost";
        public string ServerName { get; set; } = "PasswordlessApi";
        public string? Origin { get; set; }
        public List<string> AllowedOrigins { get; set; } = new() {
            "http://bijayatestui.runasp.net/",
            "https://silk-shaky-hedging.ngrok-free.dev/",
            "https://ray-champion-crow.ngrok-free.app/"
        };
    }
}
