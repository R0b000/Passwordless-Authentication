namespace Shared.Core.Models.Common
{
    public class PaginatedResponse<T>
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public List<T> Data { get; set; } = new();
    }
}
