using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Skeleton
{
    public partial class Skeleton : ComponentBase
    {
        [Parameter] public SkeletonVariant Variant { get; set; } = SkeletonVariant.Text;
        [Parameter] public int Lines { get; set; } = 1;
        [Parameter] public string? Width { get; set; } = "100%";
        [Parameter] public string? Height { get; set; } = "1rem";
        [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        public enum SkeletonVariant { Text, Rectangle, Circle, Image, Button }

        private string CssClass
        {
            get
            {
                var classes = new List<string> { "ui-skeleton-wrapper" };
                if (Attributes.TryGetValue("class", out var extra)) classes.Add(extra?.ToString() ?? string.Empty);
                return string.Join(" ", classes);
            }
        }

        private string Style
        {
            get
            {
                if (Variant == SkeletonVariant.Text && Lines > 1) return string.Empty;
                return $"width:{Width};height:{Height};";
            }
        }
    }
}
