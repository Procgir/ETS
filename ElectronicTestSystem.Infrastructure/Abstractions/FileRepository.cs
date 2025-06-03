using ElectronicTestSystem.SDK.Converters;

namespace ElectronicTestSystem.Infrastructure.Abstractions;

public abstract class FileRepository<T>(
    string fileName,
    Converter<T> converter)
{
    public string FileName { get; } = fileName;

    public Dictionary<string, T> GetDictItems()
    {
        if (!File.Exists(FileName))
        {
            return new Dictionary<string, T>(0);
        }
        
        string rawTextTests = File.ReadAllText(FileName);

        return converter.DeserializeDictItems(rawTextTests);
    }

    public List<T> GetAll()
    {
        return GetDictItems()
            .Select(p => p.Value)
            .ToList();
    }

    public void Save(T item)
    {
        var dictItems = GetDictItems();
        dictItems.Add(converter.DictKey(item), item);
        
        SaveDictItems(dictItems);
    }
    
    public void SaveDictItems(Dictionary<string, T> dictItems)
    {
        string text = converter.SerializeDictItems(dictItems);
        
        File.WriteAllText(FileName, text);
    }
}