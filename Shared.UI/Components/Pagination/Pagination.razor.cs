using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Shared.UI.Components.Pagination
{
    public partial class Pagination : ComponentBase
    {
        [Parameter] public int TotalPages { get; set; } = 1;
        [Parameter] public int CurrentPage { get; set; } = 1;
        [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }

        private async Task GoToPage(int p)
        {
            if (p == CurrentPage || p < 1 || p > TotalPages) return;
            CurrentPage = p;
            await CurrentPageChanged.InvokeAsync(p);
        }

        private Task FirstPage() => GoToPage(1);
        private Task PrevPage() => GoToPage(CurrentPage - 1);
        private Task NextPage() => GoToPage(CurrentPage + 1);
        private Task LastPage() => GoToPage(TotalPages);

        private List<int> GetPages()
        {
            var pages = new List<int>();
            if (TotalPages <= 7)
            {
                for (int i = 1; i <= TotalPages; i++) pages.Add(i);
            }
            else
            {
                pages.Add(1);
                if (CurrentPage > 3) pages.Add(-1);
                var start = Math.Max(2, CurrentPage - 1);
                var end = Math.Min(TotalPages - 1, CurrentPage + 1);
                for (int i = start; i <= end; i++) pages.Add(i);
                if (CurrentPage < TotalPages - 2) pages.Add(-1);
                pages.Add(TotalPages);
            }
            return pages;
        }
    }
}
