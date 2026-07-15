using Microsoft.AspNetCore.Components;

namespace Auth.UI.Shared.Components.Timeline;

public class TimelineItem
{
    public string? Time { get; set; }
    public string? Title { get; set; }
    public string? Color { get; set; }
    public RenderFragment? Content { get; set; }
}
