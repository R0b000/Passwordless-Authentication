namespace Shared.UI.Components.Loader;

public class LoaderService
{
    public bool IsLoading { get; private set; }
    public event Action? OnStateChange;

    public void Show()
    {
        IsLoading = true;
        OnStateChange?.Invoke();
    }

    public void Hide()
    {
        IsLoading = false;
        OnStateChange?.Invoke();
    }
}
