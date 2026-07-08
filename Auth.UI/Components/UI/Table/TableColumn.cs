using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Table;

public class TableColumn<TItem>
{
    public string Title { get; set; } = string.Empty;
    public string? Property { get; set; }
    public bool Sortable { get; set; } = true;
    public bool Visible { get; set; } = true;
    public string? Format { get; set; }

    public RenderFragment<TItem>? Template { get; set; }
}
