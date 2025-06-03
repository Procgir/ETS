using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Domain.Testings;

public static class TestingsGetOptions
{
    public static GetOptions Paged(int pageSize, int pageNumber) => CommonGetOptionsKeys.Paged(pageSize, pageNumber);
}