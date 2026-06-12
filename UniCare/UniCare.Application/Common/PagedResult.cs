namespace UniCare.Application.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;

    public static PagedResult<T> Create(IReadOnlyList<T> items, int currentPage, int pageSize, int totalCount)
        => new()
        {
            Items = items,
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
