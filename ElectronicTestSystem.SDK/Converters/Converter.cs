using System.Text;

namespace ElectronicTestSystem.SDK.Converters;

public abstract class Converter<T>
{
    protected const char SeparatorSymbol = ',';

    public abstract string DictKey(T item);
    public abstract string Serialize(T item);
    public abstract T Deserialize(string item);

    public string SerializeDictItems(Dictionary<string, T> dictItems)
    {
        StringBuilder builder = new StringBuilder();
        
        builder.AppendJoin(Environment.NewLine, 
            dictItems.Select(pair => Serialize(pair.Value)));
        
        return builder.ToString();
    }

    public Dictionary<string, T> DeserializeDictItems(string rawText)
    {
        string[] rawTextTests = rawText.Split(Environment.NewLine);
        
        var testsCorrectAnswers = rawTextTests
            .Select(Deserialize)
            .ToDictionary(t => DictKey(t), g => g);
        
        return testsCorrectAnswers;
    }
}