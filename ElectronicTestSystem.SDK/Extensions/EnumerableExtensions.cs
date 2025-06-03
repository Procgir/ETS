namespace ElectronicTestSystem.SDK.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, i) => (item, i));
    }
}