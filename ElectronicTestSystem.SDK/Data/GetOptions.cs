namespace ElectronicTestSystem.SDK.Data;

public class GetOptions
{
    private readonly Dictionary<NamedId, object> values;

    private GetOptions(Dictionary<NamedId, object> values)
    {
        this.values = values;
    }

    public static GetOptions Default => new (new Dictionary<NamedId, object>());

    public GetOptions Add<T>(OptionKey<T> key, T value)
    {
        values.Add(key.Id, value);
        return this;
    } 
    
    public T Get<T>(OptionKey<T> key) => (T)values[key.Id];
}