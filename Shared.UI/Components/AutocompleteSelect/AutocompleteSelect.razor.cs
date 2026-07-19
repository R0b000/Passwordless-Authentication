using Shared.UI.Components.AutocompleteSelect;
using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.AutocompleteSelect;

public partial class AutocompleteSelect : ComponentBase
{
    [Parameter] public List<SelectItem> Data { get; set; } = new();
    [Parameter] public bool EnableSearch { get; set; }
    [Parameter] public bool AllSelector { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Select an option";
    [Parameter] public bool Disabled { get; set; }

    [Parameter] public string SelectedId { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> SelectedIdChanged { get; set; }

    [Parameter] public EventCallback<SelectItem> OnSelected { get; set; }

    protected bool IsOpen { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected SelectItem? SelectedItem
        => Data.FirstOrDefault(d => d.Id == SelectedId);

    protected List<SelectItem> FilteredOptions
    {
        get
        {
            var options = new List<SelectItem>();
            if (AllSelector) options.Add(new SelectItem { Id = string.Empty, Value = "All" });
            options.AddRange(Data);

            if (EnableSearch && !string.IsNullOrWhiteSpace(SearchText))
            {
                options = options
                    .Where(o => o.Value.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return options;
        }
    }

    protected void Toggle()
    {
        if (Disabled) return;
        IsOpen = !IsOpen;
    }

    protected void Close()
    {
        IsOpen = false;
        SearchText = string.Empty;
    }

    protected async Task Select(SelectItem option)
    {
        SelectedId = option.Id;
        await SelectedIdChanged.InvokeAsync(option.Id);
        await OnSelected.InvokeAsync(option);
        Close();
    }
}
