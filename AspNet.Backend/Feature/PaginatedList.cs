namespace AspNet.Backend.Feature;

/// <summary>
/// The <see cref="PaginatedList{T}"/> class
/// adds pagination data to a wrapped <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="T">The generic.</typeparam>
public class PaginatedList<T>(IEnumerable<T> items, int pageIndex, int totalCount)
{
    public IEnumerable<T> Items { get; } = items;
    public int PageIndex { get; } = pageIndex;
    public int TotalCount { get; } = totalCount;
}