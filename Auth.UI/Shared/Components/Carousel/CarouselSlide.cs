using Microsoft.AspNetCore.Components;

namespace Auth.UI.Shared.Components.Carousel;

public class CarouselSlide
{
    public string? ImageUrl { get; set; }
    public string Caption { get; set; } = string.Empty;
    public RenderFragment? Content { get; set; }
}
