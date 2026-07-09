using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading;

namespace Auth.UI.Components.UI.Carousel
{
    public partial class Carousel : ComponentBase
    {
        [Parameter] public List<CarouselSlide> Slides { get; set; } = new();
        [Parameter] public int ActiveIndex { get; set; } = 0;
        [Parameter] public EventCallback<int> ActiveIndexChanged { get; set; }
        [Parameter] public bool AutoPlay { get; set; } = false;
        [Parameter] public int Interval { get; set; } = 3000;

        private ElementReference _root;
        private Timer? _timer;

        protected override void OnParametersSet()
        {
            if (ActiveIndex < 0) ActiveIndex = 0;
            if (ActiveIndex >= Slides.Count) ActiveIndex = Slides.Count - 1;
        }

        protected override void OnAfterRender(bool first)
        {
            if (AutoPlay && Slides.Count > 1)
            {
                _timer?.Dispose();
                _timer = new Timer(_ => InvokeAsync(() => Next()), null, Interval, Interval);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task Prev()
        {
            if (ActiveIndex > 0)
            {
                ActiveIndex--;
                await ActiveIndexChanged.InvokeAsync(ActiveIndex);
            }
        }

        private async Task Next()
        {
            if (ActiveIndex < Slides.Count - 1)
            {
                ActiveIndex++;
                await ActiveIndexChanged.InvokeAsync(ActiveIndex);
            }
            else
            {
                ActiveIndex = 0;
                await ActiveIndexChanged.InvokeAsync(ActiveIndex);
            }
        }

        private async Task GoTo(int idx)
        {
            if (idx < 0 || idx >= Slides.Count) return;
            ActiveIndex = idx;
            await ActiveIndexChanged.InvokeAsync(ActiveIndex);
        }
    }
}
