using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Breadcrumb;

public partial class Breadcrumb : ComponentBase
{
    [Parameter] public List<BreadcrumbItem> Items { get; set; } = new();
}

public class BreadcrumbItem
{
    public string Text { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
}
