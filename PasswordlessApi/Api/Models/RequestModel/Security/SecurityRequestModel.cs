namespace PasswordlessApi.Api.Models.RequestModel.Security
{
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class DeviceInfoRequest
    {
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class RevokeAllSessionsRequest
    {
        public string? Password { get; set; }
    }
}