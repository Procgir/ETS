namespace ElectronicTestSystem.SDK.Data;

public record PagedResult<T>(T[] Result, PagerInfo PagerInfo);
