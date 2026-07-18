using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Shared.UI.Components.Button;

public partial class Button : ComponentBase
{
    public enum ButtonVariant
    {
        Primary, Secondary, Success, Danger, Warning, Info, Light, Dark, Link
    }

    public enum ButtonSize { Small, Medium, Large }

    [Parameter] public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;
    [Parameter] public ButtonSize Size { get; set; } = ButtonSize.Medium;
    [Parameter] public string? IconName { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Block { get; set; }
    [Parameter] public bool Outline { get; set; }
    [Parameter] public string HtmlType { get; set; } = "button";
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

    private string CssClass
    {
        get
        {
            var variant = Outline ? $"outline-{Variant.ToString().ToLowerInvariant()}" : Variant.ToString().ToLowerInvariant();
            var size = Size switch
            {
                ButtonSize.Small => "sm",
                ButtonSize.Large => "lg",
                _ => ""
            };
            var classes = new List<string> { "ui-btn", $"ui-btn-{variant}" };
            if (!string.IsNullOrWhiteSpace(size)) classes.Add($"ui-btn-{size}");
            if (Block) classes.Add("ui-btn-block");
            if (Attributes.TryGetValue("class", out var extra)) classes.Add(extra?.ToString() ?? string.Empty);
            return string.Join(" ", classes);
        }
    }

    private async Task OnClickHandler(MouseEventArgs e)
    {
        if (Disabled) return;
        await OnClick.InvokeAsync(e);
    }
}
