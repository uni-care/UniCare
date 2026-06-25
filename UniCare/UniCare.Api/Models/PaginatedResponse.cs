using UniCare.Application.Common;

namespace UniCare.Api.Models
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public static PaginatedResponse<T> FromPaginatedList(PaginatedList<T> source)
        {
            return new PaginatedResponse<T>
            {
                Items = source.Items,
                PageNumber = source.PageNumber,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                TotalPages = source.TotalPages,
                HasPreviousPage = source.HasPreviousPage,
                HasNextPage = source.HasNextPage
            };
        }
    }
}