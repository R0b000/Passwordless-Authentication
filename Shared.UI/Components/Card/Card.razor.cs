using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Card
{
    public partial class Card : ComponentBase
    {
        [Parameter] public string? Title { get; set; }
        [Parameter] public string? Text { get; set; }
        [Parameter] public string? ImageUrl { get; set; }
        [Parameter] public string? ImageAlt { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public RenderFragment? Footer { get; set; }
    }
}
