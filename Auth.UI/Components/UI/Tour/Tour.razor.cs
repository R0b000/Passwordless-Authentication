using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Auth.UI.Components.UI.Tour
{
    public partial class Tour : ComponentBase
    {
        [Parameter] public bool Visible { get; set; }
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int Step { get; set; } = 0;
        [Parameter] public int TotalSteps { get; set; } = 1;
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnNext { get; set; }
        [Parameter] public EventCallback OnPrev { get; set; }

        private bool IsLast => Step >= TotalSteps - 1;
        private string PositionStyle => "top: 20%; left: 50%; transform: translate(-50%, 0);";

        private async Task Next()
        {
            await OnNext.InvokeAsync();
        }

        private async Task Prev()
        {
            await OnPrev.InvokeAsync();
        }

        private async Task Close()
        {
            Visible = false;
            await OnClose.InvokeAsync();
        }
    }
}
