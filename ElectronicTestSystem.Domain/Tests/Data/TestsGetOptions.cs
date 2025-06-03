using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Domain.Tests.Data;

public static class TestsGetOptions
{
    public static GetOptions Paged(int pageSize, int pageNumber) => CommonGetOptionsKeys.Paged(pageSize, pageNumber);
}