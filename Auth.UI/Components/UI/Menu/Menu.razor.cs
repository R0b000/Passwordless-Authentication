using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Menu
{
    public partial class Menu : ComponentBase
    {
        [Parameter] public string? Header { get; set; }
        [Parameter] public List<object> Items { get; set; } = new();
        [Parameter] public EventCallback<MenuActionItem> OnAction { get; set; }

        private async Task OnItemClicked(MenuActionItem item)
        {
            if (item.Disabled) return;
            await OnAction.InvokeAsync(item);
        }
    }
}
