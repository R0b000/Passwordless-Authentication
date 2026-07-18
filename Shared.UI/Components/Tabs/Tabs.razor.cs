using Shared.UI.Components.Tabs;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Shared.UI.Components.Tabs
{
    public partial class Tabs : ComponentBase
    {
        [Parameter] public List<TabItem> TabItems { get; set; } = new();
        [Parameter] public TabItem ActiveTab { get; set; } = new();
        [Parameter] public EventCallback<TabItem> ActiveTabChanged { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private void Activate(TabItem tab)
        {
            if (tab.Disabled) return;
            ActiveTab = tab;
            ActiveTabChanged.InvokeAsync(tab);
        }
    }
}
