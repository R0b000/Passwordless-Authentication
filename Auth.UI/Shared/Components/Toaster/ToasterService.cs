namespace Auth.UI.Shared.Components.Toaster;

public enum ToastType { Success, Warning, Danger, Info }

public enum ToastPosition
{
    TopRight, TopLeft, TopCenter, BottomRight, BottomLeft, BottomCenter
}

public class Toast
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Message { get; init; } = string.Empty;
    public ToastType Type { get; init; } = ToastType.Info;
    public ToastPosition Position { get; init; } = ToastPosition.TopRight;
    public int DurationMs { get; init; } = 4000;
}

public class ToasterService
{
    private readonly List<Toast> _toasts = new();
    public IReadOnlyList<Toast> Toasts => _toasts.AsReadOnly();

    public event Action? OnChanged;

    public void Show(string message, ToastType type = ToastType.Info,
        ToastPosition position = ToastPosition.TopRight, int durationMs = 4000)
    {
        _toasts.Add(new Toast
        {
            Message = message,
            Type = type,
            Position = position,
            DurationMs = durationMs
        });
        OnChanged?.Invoke();
    }

    public void ShowSuccess(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastType.Success, position);

    public void ShowWarning(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastType.Warning, position);

    public void ShowDanger(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastType.Danger, position);

    public void ShowInfo(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastType.Info, position);

    public void Remove(Guid id)
    {
        var removed = _toasts.RemoveAll(t => t.Id == id) > 0;
        if (removed) OnChanged?.Invoke();
    }
}
