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
            var baseClass = "inline-flex items-center justify-center gap-2 rounded-xl font-semibold transition-all focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed";
            
            var sizeClass = Size switch
            {
                ButtonSize.Small => "px-3 py-1.5 text-xs",
                ButtonSize.Large => "px-6 py-3 text-base",
                _ => "px-4 py-2 text-sm" // Medium
            };

            var variantClass = "";
            if (Outline)
            {
                variantClass = Variant switch
                {
                    ButtonVariant.Primary => "border border-emerald-800 text-emerald-800 hover:bg-emerald-50 focus:ring-emerald-500",
                    ButtonVariant.Secondary => "border border-gray-300 text-gray-700 hover:bg-gray-50 focus:ring-gray-300",
                    ButtonVariant.Success => "border border-green-600 text-green-600 hover:bg-green-50 focus:ring-green-500",
                    ButtonVariant.Danger => "border border-red-500 text-red-500 hover:bg-red-50 focus:ring-red-500",
                    ButtonVariant.Warning => "border border-amber-500 text-amber-600 hover:bg-amber-50 focus:ring-amber-500",
                    ButtonVariant.Info => "border border-blue-500 text-blue-500 hover:bg-blue-50 focus:ring-blue-500",
                    ButtonVariant.Light => "border border-gray-200 text-gray-500 hover:bg-gray-50 focus:ring-gray-100",
                    ButtonVariant.Dark => "border border-gray-900 text-gray-900 hover:bg-gray-50 focus:ring-gray-900",
                    _ => "text-emerald-800 hover:underline focus:ring-emerald-500" // Link
                };
            }
            else
            {
                variantClass = Variant switch
                {
                    ButtonVariant.Primary => "bg-emerald-800 hover:bg-emerald-900 text-white focus:ring-emerald-500",
                    ButtonVariant.Secondary => "bg-gray-100 hover:bg-gray-250 text-gray-700 focus:ring-gray-300",
                    ButtonVariant.Success => "bg-green-600 hover:bg-green-700 text-white focus:ring-green-500",
                    ButtonVariant.Danger => "bg-red-650 hover:bg-red-750 text-white focus:ring-red-500",
                    ButtonVariant.Warning => "bg-amber-500 hover:bg-amber-650 text-white focus:ring-amber-500",
                    ButtonVariant.Info => "bg-blue-600 hover:bg-blue-700 text-white focus:ring-blue-500",
                    ButtonVariant.Light => "bg-white border border-gray-200 hover:bg-gray-50 text-gray-700 focus:ring-gray-100",
                    ButtonVariant.Dark => "bg-gray-900 hover:bg-black text-white focus:ring-gray-900",
                    _ => "bg-transparent text-emerald-850 hover:underline hover:text-emerald-950 px-0 py-0" // Link
                };
            }

            var classes = new List<string> { baseClass, sizeClass, variantClass };
            if (Block) classes.Add("w-full");
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
