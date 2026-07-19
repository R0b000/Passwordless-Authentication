using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Progress
{
    public partial class Progress : ComponentBase
    {
        [Parameter] public double Value { get; set; }
        [Parameter] public double Max { get; set; } = 100;
        [Parameter] public bool ShowLabel { get; set; } = false;
        [Parameter] public string? Label { get; set; }

        private double Percent => Max <= 0 ? 0 : Math.Clamp((Value / Max) * 100, 0, 100);
        private string LabelText => !string.IsNullOrWhiteSpace(Label) ? Label : $"{Percent:F0}%";
    }
}
