namespace PasswordlessApi.Api.Models.ResponseModel.Security
{
    public class DeviceSessionResponse
    {
        public int Id { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ActiveSessionsResponse
    {
        public List<DeviceSessionResponse> Sessions { get; set; } = new();
        public int MaxAllowedSessions { get; set; } = 5;
    }
}