namespace Shared.UI.Components.Toaster;

public enum ToastLevel { Success, Warning, Danger, Info }

public static class ToastType
{
    public static Toast Success(string message, ToastPosition position = ToastPosition.TopRight, int durationMs = 3000)
        => new Toast { Message = message, Type = ToastLevel.Success, Position = position, DurationMs = durationMs };

    public static Toast Warning(string message, ToastPosition position = ToastPosition.TopRight, int durationMs = 3000)
        => new Toast { Message = message, Type = ToastLevel.Warning, Position = position, DurationMs = durationMs };

    public static Toast Danger(string message, ToastPosition position = ToastPosition.TopRight, int durationMs = 3000)
        => new Toast { Message = message, Type = ToastLevel.Danger, Position = position, DurationMs = durationMs };

    public static Toast Info(string message, ToastPosition position = ToastPosition.TopRight, int durationMs = 3000)
        => new Toast { Message = message, Type = ToastLevel.Info, Position = position, DurationMs = durationMs };
}

public enum ToastPosition
{
    TopRight, TopLeft, TopCenter, BottomRight, BottomLeft, BottomCenter
}

public class Toast
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Message { get; init; } = string.Empty;
    public ToastLevel Type { get; init; } = ToastLevel.Info;
    public ToastPosition Position { get; init; } = ToastPosition.TopRight;
    public int DurationMs { get; init; } = 3000;
}

public class ToasterService
{
    public static ToasterService? Current { get; private set; }

    private readonly List<Toast> _toasts = new();
    public IReadOnlyList<Toast> Toasts => _toasts.AsReadOnly();

    public event Action? OnChanged;

    public ToasterService()
    {
        Current = this;
    }

    public void Show(string message, ToastLevel type = ToastLevel.Info,
        ToastPosition position = ToastPosition.TopRight, int durationMs = 3000)
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
        => Show(message, ToastLevel.Success, position);

    public void ShowWarning(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastLevel.Warning, position);

    public void ShowDanger(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastLevel.Danger, position);

    public void ShowInfo(string message, ToastPosition position = ToastPosition.TopRight)
        => Show(message, ToastLevel.Info, position);

    public void Notify(Toast toast)
    {
        _toasts.Add(toast);
        OnChanged?.Invoke();
    }

    public void Remove(Guid id)
    {
        var removed = _toasts.RemoveAll(t => t.Id == id) > 0;
        if (removed) OnChanged?.Invoke();
    }
}
