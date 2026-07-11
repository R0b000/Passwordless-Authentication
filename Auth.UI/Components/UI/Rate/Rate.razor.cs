using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Rate
{
    public partial class Rate : ComponentBase
    {
        [Parameter] public int Max { get; set; } = 5;
        [Parameter] public int CurrentValue { get; set; }
        [Parameter] public EventCallback<int> CurrentValueChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool AllowClear { get; set; } = true;
        [Parameter] public bool ShowText { get; set; } = false;
        [Parameter] public string? Label { get; set; }

        private int? HoverValue { get; set; }

        private int DisplayValue => HoverValue ?? CurrentValue;

        private async Task SetValue(int value)
        {
            if (Disabled) return;
            if (AllowClear && CurrentValue == value)
            {
                CurrentValue = 0;
                await CurrentValueChanged.InvokeAsync(0);
            }
            else
            {
                CurrentValue = value;
                await CurrentValueChanged.InvokeAsync(value);
            }
        }
    }
}
