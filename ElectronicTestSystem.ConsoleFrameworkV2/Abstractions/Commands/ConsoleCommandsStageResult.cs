namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

public class ConsoleCommandsStageResult(List<string> inputStrings, List<string> outputStrings)
{
    public List<string> InputStrings { get; } = inputStrings;
    public List<string> OutputStrings { get; } = outputStrings;

    public static ConsoleCommandsStageResult Empty() 
        => new (new List<string>(0), new List<string>(0));
}