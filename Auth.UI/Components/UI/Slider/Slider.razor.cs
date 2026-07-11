using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Slider
{
    public partial class Slider : ComponentBase
    {
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        [Parameter] public double Min { get; set; } = 0;
        [Parameter] public double Max { get; set; } = 100;
        [Parameter] public double Value { get; set; }
        [Parameter] public EventCallback<double> ValueChanged { get; set; }
        [Parameter] public double Step { get; set; } = 1;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ShowSteps { get; set; } = false;
        [Parameter] public string? Label { get; set; }

        private ElementReference _trackElement;
        private bool _isDragging;

        private double FillPercent => (Value - Min) / (Max - Min) * 100;

        private double SnapValue(double rawValue)
        {
            if (Step <= 0) return Math.Clamp(rawValue, Min, Max);
            var stepped = Math.Round((rawValue - Min) / Step) * Step + Min;
            return Math.Clamp(stepped, Min, Max);
        }

        private async Task UpdateValueFromPosition(double clientX)
        {
            if (Disabled) return;

            var bounds = await JsRuntime.InvokeAsync<ElementRect>("getElementRect", _trackElement);
            var ratio = (clientX - bounds.Left) / bounds.Width;
            ratio = Math.Clamp(ratio, 0, 1);
            var rawValue = Min + ratio * (Max - Min);
            var newValue = SnapValue(rawValue);

            if (!(Math.Abs(newValue - Value) > 0.0001)) return;

            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

        private async Task OnMouseDown(MouseEventArgs e)
        {
            if (Disabled) return;
            _isDragging = true;
            await UpdateValueFromPosition(e.ClientX);

            _ = Task.Run(async () =>
            {
                await Task.Delay(1);
                if (_isDragging)
                {
                    await InvokeAsync(async () =>
                    {
                        if (_isDragging)
                        {
                            await UpdateValueFromPosition(e.ClientX);
                        }
                    });
                }
            });
        }

        private async Task OnTouchStart(TouchEventArgs e)
        {
            if (Disabled) return;
            _isDragging = true;

            if (e.Touches.Length > 0)
            {
                await UpdateValueFromPosition(e.Touches[0].ClientX);
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(1);
                if (_isDragging)
                {
                    await InvokeAsync(async () =>
                    {
                        if (_isDragging)
                        {
                            if (e.Touches.Length > 0)
                            {
                                await UpdateValueFromPosition(e.Touches[0].ClientX);
                            }
                        }
                    });
                }
            });
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (Disabled) return;
            var step = Step;
            var newValue = Value;

            if (e.Key == "ArrowRight" || e.Key == "ArrowUp") newValue = SnapValue(Value + step);
            else if (e.Key == "ArrowLeft" || e.Key == "ArrowDown") newValue = SnapValue(Value - step);
            else if (e.Key == "Home") newValue = Min;
            else if (e.Key == "End") newValue = Max;
            else if (e.Key == "PageUp") newValue = SnapValue(Value + step * 10);
            else if (e.Key == "PageDown") newValue = SnapValue(Value - step * 10);
            else return;

            if (!(Math.Abs(newValue - Value) > 0.0001)) return;

            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

        private List<double> GetSteps()
        {
            var list = new List<double>();
            if (Step <= 0) return list;
            for (double v = Min; v <= Max; v += Step) list.Add(Math.Round(v, 6));
            return list;
        }
        private class ElementRect
        {
            public double Left { get; set; }
            public double Width { get; set; }
        }
    }
}
