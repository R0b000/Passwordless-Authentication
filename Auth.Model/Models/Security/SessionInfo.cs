namespace Auth.Model.Models.Security
{
    public class SessionInfo
    {
        public string Id { get; set; } = string.Empty;
        public string DeviceType { get; set; } = "desktop";
        public string Browser { get; set; } = string.Empty;
        public string Os { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
        public bool IsCurrent { get; set; }
    }
    
    public class ActivityQuery
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Type { get; set; }
        public string? Search { get; set; }
    }
    
    public class ActionResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
