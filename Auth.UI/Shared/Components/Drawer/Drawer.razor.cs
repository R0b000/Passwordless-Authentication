using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Drawer
{
    public partial class Drawer : ComponentBase
    {
        [Parameter] public bool Visible { get; set; }
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
        [Parameter] public DrawerPlacement Placement { get; set; } = DrawerPlacement.Left;
        [Parameter] public EventCallback OnClose { get; set; }

        public enum DrawerPlacement { Left, Right }

        private async Task Close()
        {
            Visible = false;
            await OnClose.InvokeAsync();
        }
    }
}
