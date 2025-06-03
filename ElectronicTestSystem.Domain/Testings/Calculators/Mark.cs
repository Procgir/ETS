namespace ElectronicTestSystem.Domain.Testings.Calculators;

public class Mark
{
    private static readonly Dictionary<MarkValue, Mark> Marks = new()
    {
        {MarkValue.One, new Mark(MarkValue.One)},
        {MarkValue.Two, new Mark(MarkValue.Two)},
        {MarkValue.Three, new Mark(MarkValue.Three)},
        {MarkValue.Four, new Mark(MarkValue.Four)},
        {MarkValue.Five, new Mark(MarkValue.Five)}
    };
    
    public MarkValue Value { get; }
    
    public static Mark One => Marks[MarkValue.One];
    public static Mark Two => Marks[MarkValue.Two];
    public static Mark Three => Marks[MarkValue.Three];
    public static Mark Four => Marks[MarkValue.Four];
    public static Mark Five => Marks[MarkValue.Five];

    private Mark(MarkValue value)
    {
        Value = value;
    }
    
    public static implicit operator int(Mark mark) => (int)mark.Value;

    public override string ToString()
    {
        return ((int)Value).ToString();
    }
}