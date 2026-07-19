using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Carousel;

public class CarouselSlide
{
    public string? ImageUrl { get; set; }
    public string Caption { get; set; } = string.Empty;
    public RenderFragment? Content { get; set; }
}
