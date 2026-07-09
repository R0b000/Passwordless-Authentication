using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.TimePicker
{
    public partial class TimePicker : ComponentBase
    {
        [Parameter] public TimeOnly? SelectedTime { get; set; }
        [Parameter] public EventCallback<TimeOnly?> SelectedTimeChanged { get; set; }
        [Parameter] public bool Popup { get; set; } = false;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public TimeOnly? Min { get; set; }
        [Parameter] public TimeOnly? Max { get; set; }

        private int SelectedHour => SelectedTime?.Hour ?? 0;
        private int SelectedMinute => SelectedTime?.Minute ?? 0;

        private async Task SetHour(int hour)
        {
            var ts = new TimeOnly(hour, SelectedMinute, 0);
            SelectedTime = ts;
            await SelectedTimeChanged.InvokeAsync(SelectedTime);
        }

        private async Task SetMinute(int minute)
        {
            var ts = new TimeOnly(SelectedHour, minute, 0);
            SelectedTime = ts;
            await SelectedTimeChanged.InvokeAsync(SelectedTime);
        }
    }
}
