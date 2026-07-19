using Microsoft.AspNetCore.Components;
using global::Shared.UI.Components.Timeline;
using global::Shared.UI.Components.Toaster;
using global::Shared.Core.UIModels.Security;
using global::Shared.UI.Manager.Interface.Auth;

namespace Auth.UI.Components.Pages.Customer.Account.Activity
{
    public partial class Activity : ComponentBase
    {
        [Inject] private ISecurityManager SecurityManager { get; set; } = default!;
        [Inject] private ToasterService Toaster { get; set; } = default!;

        protected ActivityQuery Query { get; set; } = new();
        protected List<ActivityLogEntry> Entries { get; set; } = new();

        protected List<TimelineItem> TimelineItems => Entries.Select(e => new TimelineItem
        {
            Title = e.Description,
            Time = e.Timestamp.ToString("g"),
            Content = builder => builder.AddContent(0, $"{e.Device} · {e.IpAddress}"),
            Color = TypeColor(e.Type)
        }).ToList();

        protected override async Task OnInitializedAsync()
        {
            await ApplyFiltersAsync();
        }

        protected async Task ApplyFiltersAsync()
        {
            var result = await SecurityManager.GetActivityAsync(Query);
            Entries = result.Succeeded ? result.Data ?? new List<ActivityLogEntry>() : new List<ActivityLogEntry>();
        }

        protected async Task ResetFiltersAsync()
        {
            Query = new ActivityQuery();
            await ApplyFiltersAsync();
        }

        private static string TypeColor(string type) => type switch
        {
            "login" => "#2f9e44",
            "logout" => "#868e96",
            "password_change" => "#f08c00",
            "settings_update" => "#1971c2",
            "2fa" => "#6741d9",
            "login_failed" => "#e03131",
            _ => "#2f9e44"
        };
    }
}
