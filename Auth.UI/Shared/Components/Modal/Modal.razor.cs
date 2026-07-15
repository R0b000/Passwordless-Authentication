using Auth.UI.Shared.Components.Modal;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Modal;

public partial class Modal : ComponentBase
{
    [Parameter] public ModalSize Size { get; set; } = ModalSize.Medium;
    [Parameter] public string Title { get; set; } = "Modal";
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter] public string SubmitText { get; set; } = "Submit";
    [Parameter] public string SubmitIcon { get; set; } = "check";
    [Parameter] public bool ShowSubmitButton { get; set; } = true;
    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public bool CloseOnOverlayClick { get; set; } = true;

    [Parameter] public EventCallback OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    protected bool IsSubmitting { get; set; }

    public async Task ShowAsync()
    {
        Visible = true;
        await VisibleChanged.InvokeAsync(true);
        await InvokeAsync(StateHasChanged);
    }

    public async Task HideAsync()
    {
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
        await InvokeAsync(StateHasChanged);
    }

    protected async Task SubmitAsync()
    {
        if (!ShowSubmitButton) return;
        IsSubmitting = true;
        await InvokeAsync(StateHasChanged);
        try
        {
            await OnSubmit.InvokeAsync();
        }
        finally
        {
            IsSubmitting = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task CancelAsync()
    {
        await OnCancel.InvokeAsync();
        await HideAsync();
    }

    private async Task OverlayClick()
    {
        if (CloseOnOverlayClick) await CancelAsync();
    }
}
