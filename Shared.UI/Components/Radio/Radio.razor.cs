using Microsoft.AspNetCore.Components;
using System;

namespace Shared.UI.Components.Radio
{
    public partial class Radio : ComponentBase
    {
        [Parameter] public string GroupName { get; set; } = Guid.NewGuid().ToString();
        [Parameter] public string? Value { get; set; }
        [Parameter] public string? SelectedValue { get; set; }
        [Parameter] public EventCallback<string?> SelectedValueChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private bool IsChecked => SelectedValue == Value;

        private async Task OnChanged(ChangeEventArgs e)
        {
            if (Disabled) return;
            SelectedValue = Value;
            await SelectedValueChanged.InvokeAsync(SelectedValue);
        }
    }
}
