using Auth.UI.Shared.Components.Steps;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Steps
{
    public partial class Steps : ComponentBase
    {
        [Parameter] public List<StepItem> Items { get; set; } = new();
        [Parameter] public int ActiveIndex { get; set; } = 0;

        private string GetStatusClass(int index)
        {
            if (index < ActiveIndex) return "is-completed";
            if (index == ActiveIndex) return "is-active";
            return "";
        }

        private string GetIndicator(int index)
        {
            if (index < ActiveIndex) return "✓";
            return $"{index + 1}";
        }
    }
}
