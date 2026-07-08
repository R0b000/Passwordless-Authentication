using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Reflection;

namespace Auth.UI.Components.UI.Table;

public partial class Table<TItem> : ComponentBase
{
    [Parameter] public List<TItem> Items { get; set; } = new();
    [Parameter] public List<TableColumn<TItem>> Columns { get; set; } = new();
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public bool EnableFiltering { get; set; } = true;
    [Parameter] public bool EnableColumnToggle { get; set; } = true;
    [Parameter] public bool EnablePagination { get; set; } = true;
    [Parameter] public bool EnableSorting { get; set; } = true;
    [Parameter] public RenderFragment? ChildToolbar { get; set; }

    protected List<TableColumn<TItem>> VisibleColumns => Columns.Where(c => c.Visible).ToList();

    protected List<FilterRow> Filters { get; set; } = new();
    protected bool ShowFilters { get; set; }
    protected bool ShowColumnToggle { get; set; }

    protected string? SortColumn { get; set; }
    protected bool SortDescending { get; set; }

    protected int CurrentPage { get; set; } = 1;
    protected List<int> PageSizes { get; set; } = new() { 5, 10, 25, 50, 100 };

    private List<TItem> _filtered = new();
    protected List<TItem> FilteredItems => _filtered;

    private List<TItem> _paged = new();
    protected List<TItem> PagedItems => _paged;

    protected int TotalPages => Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)PageSize));
    protected int StartIndex => _filtered.Count == 0 ? 0 : (CurrentPage - 1) * PageSize;
    protected int EndIndex => Math.Min(CurrentPage * PageSize, _filtered.Count);

    protected override void OnParametersSet() => Refresh();

    private void Refresh()
    {
        var query = Items ?? new List<TItem>();

        var active = Filters.Where(f => !string.IsNullOrEmpty(f.Property)).ToList();
        if (active.Count > 0)
        {
            query = query.Where(item => active.All(f => EvaluateFilter(item, f))).ToList();
        }

        if (EnableSorting && !string.IsNullOrEmpty(SortColumn))
        {
            query = Sort(query);
        }

        _filtered = query;

        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        if (CurrentPage < 1) CurrentPage = 1;

        _paged = EnablePagination
            ? _filtered.Skip(StartIndex).Take(PageSize).ToList()
            : _filtered;
    }

    private bool EvaluateFilter(TItem item, FilterRow f)
        => ApplyOperator(GetValue(item, f.Property!), f.Operator, f.Value);

    private bool ApplyOperator(object? left, FilterOperator op, string? right)
    {
        switch (op)
        {
            case FilterOperator.IsEmpty:
                return left is null || string.IsNullOrWhiteSpace(left.ToString());
            case FilterOperator.IsNotEmpty:
                return !(left is null || string.IsNullOrWhiteSpace(left.ToString()));
            case FilterOperator.Contains:
                return left?.ToString()?.Contains(right ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
            case FilterOperator.NotContains:
                return !(left?.ToString()?.Contains(right ?? "", StringComparison.OrdinalIgnoreCase) ?? false);
            case FilterOperator.StartsWith:
                return left?.ToString()?.StartsWith(right ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
            case FilterOperator.EndsWith:
                return left?.ToString()?.EndsWith(right ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
            default:
                var cmp = CompareValues(left, right);
                return op switch
                {
                    FilterOperator.Equals => cmp == 0,
                    FilterOperator.NotEquals => cmp != 0,
                    FilterOperator.GreaterThan => cmp > 0,
                    FilterOperator.GreaterThanOrEqual => cmp >= 0,
                    FilterOperator.LessThan => cmp < 0,
                    FilterOperator.LessThanOrEqual => cmp <= 0,
                    _ => true
                };
        }
    }

    private int CompareValues(object? left, string? right)
    {
        if (left is null && right is null) return 0;
        if (left is null) return -1;
        if (right is null) return 1;

        var leftType = Nullable.GetUnderlyingType(left.GetType()) ?? left.GetType();
        object? converted;
        try { converted = Convert.ChangeType(right, leftType, CultureInfo.InvariantCulture); }
        catch { converted = right; }

        if (converted is IComparable && left is IComparable)
        {
            return ((IComparable)left).CompareTo(converted);
        }
        return string.Compare(left.ToString(), right, StringComparison.OrdinalIgnoreCase);
    }

    private List<TItem> Sort(List<TItem> query)
    {
        query.Sort((a, b) =>
        {
            var c = CompareValues(GetValue(a, SortColumn!), GetValue(b, SortColumn!)?.ToString());
            return SortDescending ? -c : c;
        });
        return query;
    }

    private static object? GetValue(TItem item, string property)
        => item?.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance)?.GetValue(item);

    private static string FormatValue(TItem item, TableColumn<TItem> col)
    {
        if (col.Property is null) return string.Empty;
        var v = GetValue(item, col.Property);
        if (v is null) return string.Empty;
        if (!string.IsNullOrEmpty(col.Format) && v is IFormattable formattable)
        {
            return formattable.ToString(col.Format, CultureInfo.CurrentCulture);
        }
        return v.ToString() ?? string.Empty;
    }

    protected void SortBy(TableColumn<TItem> col)
    {
        if (!EnableSorting || !col.Sortable || col.Property is null) return;
        if (SortColumn == col.Property) SortDescending = !SortDescending;
        else { SortColumn = col.Property; SortDescending = false; }
        CurrentPage = 1;
    }

    protected void ToggleColumn(TableColumn<TItem> col, ChangeEventArgs e)
    {
        col.Visible = e.Value is not null && bool.TryParse(e.Value.ToString(), out var v) && v;
    }

    protected void AddFilter() => Filters.Add(new FilterRow { Operator = FilterOperator.Equals });
    protected void RemoveFilter(FilterRow f) { Filters.Remove(f); CurrentPage = 1; }
    protected void ClearFilters() { Filters.Clear(); CurrentPage = 1; }

    protected void OnFilterColumnChanged(FilterRow f, ChangeEventArgs e)
    {
        f.Property = e.Value?.ToString();
        f.Value = string.Empty;
        CurrentPage = 1;
    }

    protected void OnFilterValueChanged(FilterRow f, ChangeEventArgs e)
    {
        f.Value = e.Value?.ToString();
        CurrentPage = 1;
    }

    private static FilterOperator ParseOperator(object? value)
        => Enum.TryParse<FilterOperator>(value?.ToString(), out var op) ? op : FilterOperator.Equals;

    protected void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }
    protected void PrevPage() { if (CurrentPage > 1) CurrentPage--; }
    protected void FirstPage() => CurrentPage = 1;
    protected void LastPage() => CurrentPage = TotalPages;

    protected void ChangePageSize(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size) && size > 0)
        {
            PageSize = size;
            CurrentPage = 1;
        }
    }

    public class FilterRow
    {
        public string? Property { get; set; }
        public FilterOperator Operator { get; set; } = FilterOperator.Equals;
        public string? Value { get; set; }
    }
}
