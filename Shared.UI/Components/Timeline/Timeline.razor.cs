using Shared.UI.Components.Timeline;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Shared.UI.Components.Timeline
{
    public partial class Timeline : ComponentBase
    {
        [Parameter] public List<TimelineItem> Items { get; set; } = new();
    }
}
