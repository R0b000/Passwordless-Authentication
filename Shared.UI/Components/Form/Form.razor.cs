using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Shared.UI.Components.Form
{
    public partial class Form : ComponentBase
    {
        [Parameter] public object? Model { get; set; }
        [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }
        [Parameter] public EventCallback<EditContext> OnInvalidSubmit { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool ShowActions { get; set; } = true;
        [Parameter] public bool ShowCancel { get; set; } = true;
        [Parameter] public bool IsSubmitting { get; set; }
        [Parameter] public bool IsValid { get; set; } = true;
        [Parameter] public string SubmitText { get; set; } = "Submit";
        [Parameter] public string CancelText { get; set; } = "Cancel";
        [Parameter] public EventCallback OnCancel { get; set; }
    }
}
