namespace ElectronicTestSystem.Application.Dto;

public record PagedResultResponse<T>(T[] Result, PagedResultResponsePagerInfo PagerInfo);

public record PagedResultResponsePagerInfo(int PageNumber, int PageSize, int TotalCount, int TotalPagesCount);

