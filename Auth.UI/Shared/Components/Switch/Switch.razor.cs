using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Auth.UI.Components.UI.Switch
{
    public partial class Switch : ComponentBase
    {
        [Parameter] public bool Checked { get; set; }
        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ShowLabel { get; set; } = true;
        [Parameter] public string? Label { get; set; }

        private async Task Toggle()
        {
            if (Disabled) return;
            Checked = !Checked;
            await CheckedChanged.InvokeAsync(Checked);
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (Disabled) return;
            if (e.Key == " " || e.Key == "Enter")
            {
                await Toggle();
            }
        }
    }
}
