using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Timeline
{
    public partial class Timeline : ComponentBase
    {
        [Parameter] public List<TimelineItem> Items { get; set; } = new();
    }
}
