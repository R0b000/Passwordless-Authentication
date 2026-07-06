using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Model.Auth;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject] private AuthController AuthController { get; set; } = default!;

        protected AuthResponse? CurrentUser { get; set; }
        protected string StatusMessage { get; set; } = string.Empty;
        protected bool Succeeded { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadProfileAsync();
        }

        protected async Task LoadProfileAsync()
        {
            StatusMessage = string.Empty;
            var result = await AuthController.MeAsync();
            Succeeded = result.Succeeded;
            StatusMessage = result.Succeeded ? string.Empty : (result.Message ?? "Unauthorized");
            CurrentUser = result.Succeeded ? result.Data : null;
        }
    }
}
