using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Collapse;

public class CollapseItem
{
    public string Key { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public RenderFragment? Content { get; set; }
}
