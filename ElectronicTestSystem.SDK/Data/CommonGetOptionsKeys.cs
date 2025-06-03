namespace ElectronicTestSystem.SDK.Data;

public static class CommonGetOptionsKeys
{
    private static readonly string PageSizeKey = "page-size";
    private static readonly string PageNumberKey = "page-number";
    
    public static readonly OptionKey<int> PageSize = new OptionKey<int>(new(PageSizeKey));
    public static readonly OptionKey<int> PageNumber = new OptionKey<int>(new(PageNumberKey));
    
    public static GetOptions Paged(int pageSize, int pageNumber) 
        => GetOptions.Default.Add(PageSize, pageSize).Add(PageNumber, pageNumber);
}