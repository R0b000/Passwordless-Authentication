namespace Auth.UI.Shared.Components.Table;

public enum FilterOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    IsEmpty,
    IsNotEmpty
}

public static class FilterOperatorExtensions
{
    public static string Label(this FilterOperator op) => op switch
    {
        FilterOperator.Equals => "is equal to",
        FilterOperator.NotEquals => "is not equal to",
        FilterOperator.GreaterThan => "is greater than",
        FilterOperator.GreaterThanOrEqual => "is greater than or equal to",
        FilterOperator.LessThan => "is less than",
        FilterOperator.LessThanOrEqual => "is less than or equal to",
        FilterOperator.Contains => "contains",
        FilterOperator.NotContains => "does not contain",
        FilterOperator.StartsWith => "starts with",
        FilterOperator.EndsWith => "ends with",
        FilterOperator.IsEmpty => "is empty",
        FilterOperator.IsNotEmpty => "is not empty",
        _ => op.ToString()
    };
}
