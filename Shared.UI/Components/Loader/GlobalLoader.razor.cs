using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Loader;

public partial class GlobalLoader : ComponentBase, IDisposable
{
    [Inject] private LoaderService Service { get; set; } = default!;

    protected override void OnInitialized()
    {
        Service.OnChanged += HandleStateChanged;
    }

    private void HandleStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Service.OnChanged -= HandleStateChanged;
    }
}
