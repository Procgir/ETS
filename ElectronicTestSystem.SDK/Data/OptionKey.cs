namespace ElectronicTestSystem.SDK.Data;

public record OptionKey<T>(NamedId Id) : OptionKey(Id);

public abstract record OptionKey(NamedId Id);