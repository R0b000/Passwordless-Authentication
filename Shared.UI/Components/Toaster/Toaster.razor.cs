using Shared.UI.Components.Toaster;
using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Toaster;

public partial class Toaster : ComponentBase, IDisposable
{
    [Inject] private ToasterService Service { get; set; } = default!;

    [Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;

    [Parameter] public string? Message { get; set; }
    [Parameter] public ToastType Type { get; set; } = ToastType.Info;
    [Parameter] public int DurationMs { get; set; } = 4000;

    private IReadOnlyList<Toast> Toasts => Service.Toasts.Where(t => t.Position == Position).ToList();
    private readonly HashSet<Guid> _scheduled = new();

    protected override void OnInitialized()
    {
        Service.OnChanged += StateHasChanged;
        if (!string.IsNullOrWhiteSpace(Message))
        {
            Service.Show(Message, Type, Position, DurationMs);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        foreach (var toast in Toasts)
        {
            if (_scheduled.Contains(toast.Id)) continue;
            _scheduled.Add(toast.Id);
            var id = toast.Id;
            var delay = toast.DurationMs;
            _ = Task.Delay(delay).ContinueWith(_ =>
            {
                Service.Remove(id);
                _scheduled.Remove(id);
            });
        }
        await Task.CompletedTask;
    }

    private static string IconFor(ToastType type) => type switch
    {
        ToastType.Success => "check-circle",
        ToastType.Warning => "alert-triangle",
        ToastType.Danger => "alert-circle",
        _ => "info-circle"
    };

    public void Dispose() => Service.OnChanged -= StateHasChanged;
}
