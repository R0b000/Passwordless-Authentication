using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Checkbox
{
    public partial class Checkbox : ComponentBase
    {
        [Parameter] public bool Checked { get; set; }
        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private async Task OnChanged(ChangeEventArgs e)
        {
            if (Disabled) return;
            var value = e.Value is bool b && b;
            await CheckedChanged.InvokeAsync(value);
        }
    }
}
