namespace ElectronicTestsSystem.ConsoleFramework.Shared.Components;

public class RenderInfo(int beginLeft, int beginTop, int endLeft, int endTop, List<string> inputStrings, List<string> outputStrings)
{
     public int BeginLeft { get; } = beginLeft;
     public int BeginTop { get; } = beginTop;
     public int EndLeft { get; } = endLeft;
     public int EndTop { get; } = endTop;
     public List<string> InputStrings { get; } = inputStrings;
     public List<string> OutputStrings { get; } = outputStrings;
}