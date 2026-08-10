namespace Shared.UI.Components.Loader;

public class LoaderService
{
    public bool IsLoading { get; private set; }
    public string Message { get; private set; } = "Loading...";

    public event Action? OnChanged;

    public void Show(string message = "Loading...")
    {
        IsLoading = true;
        Message = message;
        OnChanged?.Invoke();
    }

    public void Hide()
    {
        IsLoading = false;
        OnChanged?.Invoke();
    }

    public void Toggle(bool show, string message = "Loading...")
    {
        if (show) Show(message);
        else Hide();
    }
}
