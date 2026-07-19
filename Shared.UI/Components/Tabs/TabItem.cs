using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Tabs;

public class TabItem
{
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public RenderFragment? Content { get; set; }
    public bool Disabled { get; set; }
}
