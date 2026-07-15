using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Calendar
{
    public partial class Calendar : ComponentBase
    {
        [Parameter] public DateTime? SelectedDate { get; set; }
        [Parameter] public EventCallback<DateTime?> SelectedDateChanged { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public DateTime? Min { get; set; }
        [Parameter] public DateTime? Max { get; set; }
        [Parameter] public DateTime ViewDate { get; set; } = DateTime.Today;

        private string Title => ViewDate.ToString("MMMM yyyy");
        private static readonly string[] DayNames = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
        private List<DateCell> Days { get; set; } = new();

        protected override void OnParametersSet()
        {
            BuildDays();
        }

        private void BuildDays()
        {
            Days = new List<DateCell>();
            var first = new DateTime(ViewDate.Year, ViewDate.Month, 1);
            var startDay = (int)first.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(ViewDate.Year, ViewDate.Month);
            var prevMonth = ViewDate.AddMonths(-1);
            var prevDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            var today = DateTime.Today;
            var sel = SelectedDate;

            for (int i = startDay - 1; i >= 0; i--)
            {
                var d = prevDays - i;
                Days.Add(new DateCell(d, true, new DateTime(prevMonth.Year, prevMonth.Month, d), today, sel));
            }
            for (int d = 1; d <= daysInMonth; d++)
            {
                Days.Add(new DateCell(d, false, new DateTime(ViewDate.Year, ViewDate.Month, d), today, sel));
            }
            var remaining = 42 - Days.Count;
            var nextMonth = ViewDate.AddMonths(1);
            for (int d = 1; d <= remaining; d++)
            {
                Days.Add(new DateCell(d, true, new DateTime(nextMonth.Year, nextMonth.Month, d), today, sel));
            }
        }

        private void PrevMonth()
        {
            ViewDate = ViewDate.AddMonths(-1);
            BuildDays();
        }

        private void NextMonth()
        {
            ViewDate = ViewDate.AddMonths(1);
            BuildDays();
        }

        private async Task Select(DateCell cell)
        {
            if (cell.IsOtherMonth) return;
            SelectedDate = cell.Date;
            await SelectedDateChanged.InvokeAsync(SelectedDate);
            BuildDays();
        }

        private class DateCell
        {
            public int Day { get; set; }
            public bool IsOtherMonth { get; set; }
            public DateTime Date { get; set; }
            public bool IsToday { get; set; }
            public bool IsSelected { get; set; }
            public DateCell(int day, bool isOtherMonth, DateTime date, DateTime today, DateTime? selected)
            {
                Day = day; IsOtherMonth = isOtherMonth; Date = date;
                IsToday = date.Date == today.Date;
                IsSelected = selected.HasValue && date.Date == selected.Value.Date;
            }
        }
    }
}
