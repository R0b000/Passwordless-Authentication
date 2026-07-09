using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Tag
{
    public partial class Tag : ComponentBase
    {
        [Parameter] public TagVariant Variant { get; set; } = TagVariant.Primary;
        [Parameter] public string? TagIcon { get; set; }
        [Parameter] public bool Closable { get; set; } = false;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }

        public enum TagVariant { Primary, Secondary, Success, Danger, Warning, Info }

        private async Task Close()
        {
            await OnClose.InvokeAsync();
        }
    }
}
