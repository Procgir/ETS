namespace ElectronicTestSystem.SDK.Data;

public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    
    public int TotalCount { get; }
    
    public int TotalPages { get; }

    public PaginatedResult(IEnumerable<T> source, int pageNumber, int pageSize, int totalCount)
    {
        Items = source.ToList();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int) Math.Ceiling(TotalCount / (double) PageSize);
    }
}