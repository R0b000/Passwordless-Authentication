using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Auth.UI.Components.UI.Datepicker
{
    public partial class Datepicker : ComponentBase
    {
        [Parameter] public DateTime? SelectedDate { get; set; }
        [Parameter] public EventCallback<DateTime?> SelectedDateChanged { get; set; }
        [Parameter] public bool Popup { get; set; } = false;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public DateTime? Min { get; set; }
        [Parameter] public DateTime? Max { get; set; }

        private DateTime ViewMonth = DateTime.Today;
        private List<DateCell> Days = new();
        private static readonly string[] DayNames = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

        private string Title => ViewMonth.ToString("MMMM yyyy");

        protected override void OnParametersSet()
        {
            if (SelectedDate.HasValue && ViewMonth.Month != SelectedDate.Value.Month)
            {
                ViewMonth = new DateTime(SelectedDate.Value.Year, SelectedDate.Value.Month, 1);
            }
            BuildDays();
        }

        private void BuildDays()
        {
            Days = new List<DateCell>();
            var first = new DateTime(ViewMonth.Year, ViewMonth.Month, 1);
            var startDay = (int)first.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(ViewMonth.Year, ViewMonth.Month);
            var prevMonth = ViewMonth.AddMonths(-1);
            var prevMonthDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            var today = DateTime.Today;
            var sel = SelectedDate;

            for (int i = startDay - 1; i >= 0; i--)
            {
                var d = prevMonthDays - i;
                Days.Add(new DateCell(d, false, new DateTime(ViewMonth.Year, ViewMonth.Month == 1 ? 12 : ViewMonth.Month - 1, d), today, sel));
            }
            for (int d = 1; d <= daysInMonth; d++)
            {
                Days.Add(new DateCell(d, true, new DateTime(ViewMonth.Year, ViewMonth.Month, d), today, sel));
            }
            var remaining = 42 - Days.Count;
            for (int d = 1; d <= remaining; d++)
            {
                Days.Add(new DateCell(d, false, new DateTime(ViewMonth.Year, ViewMonth.Month == 12 ? 1 : ViewMonth.Month + 1, d), today, sel));
            }
        }

        private void PrevMonth()
        {
            ViewMonth = ViewMonth.AddMonths(-1);
            BuildDays();
        }

        private void NextMonth()
        {
            ViewMonth = ViewMonth.AddMonths(1);
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
