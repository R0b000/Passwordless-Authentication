using Auth.UI.Shared.Components.Collapse;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Collapse
{
    public partial class Collapse : ComponentBase
    {
        [Parameter] public List<CollapseItem> Items { get; set; } = new();
        [Parameter] public bool Accordion { get; set; } = false;
        [Parameter] public EventCallback<string> OnToggle { get; set; }

        private HashSet<string> ActiveKeys { get; set; } = new();

        private void Toggle(string key)
        {
            if (Accordion)
            {
                if (ActiveKeys.Contains(key)) ActiveKeys.Remove(key);
                else { ActiveKeys.Clear(); ActiveKeys.Add(key); }
            }
            else
            {
                if (ActiveKeys.Contains(key)) ActiveKeys.Remove(key);
                else ActiveKeys.Add(key);
            }
            OnToggle.InvokeAsync(key);
        }
    }
}
