using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Modal;

public partial class ConfirmationModal : ComponentBase
{
    [Parameter] public ModalSize Size { get; set; } = ModalSize.Small;
    [Parameter] public string Title { get; set; } = "Confirm";
    [Parameter] public string Message { get; set; } = "Are you sure?";
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter] public string ConfirmText { get; set; } = "Yes";
    [Parameter] public string CancelText { get; set; } = "No";

    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public EventCallback OnDecline { get; set; }

    private Modal _modal = default!;

    public async Task ShowAsync()
    {
        await _modal.ShowAsync();
        await VisibleChanged.InvokeAsync(true);
    }

    public async Task HideAsync()
    {
        await _modal.HideAsync();
        await VisibleChanged.InvokeAsync(false);
    }

    protected async Task ConfirmAsync()
    {
        await OnConfirm.InvokeAsync();
        await HideAsync();
    }

    protected async Task DeclineAsync()
    {
        await OnDecline.InvokeAsync();
    }
}
