namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

public class ConsoleCommandStageState(
    string stageName,
    int stageNumber,
    StageExecutingStatusType status,
    ConsoleCommandsStageResult result)
{
    public StageExecutingStatusType Status { get; internal set; } = status;
    public ConsoleCommandsStageResult Result { get; internal set; } = result;
    public string StageName { get; internal set; } = stageName;
    public int StageNumber { get; internal set; } = stageNumber;
    public bool IsFirst { get; internal set; }
    public bool IsLast { get; internal set; }
    internal int BeginTopPosition { get; set; }
    internal int EndTopPosition { get; set; }

    public static ConsoleCommandStageState Failed(string stageName, int stageNumber, ConsoleCommandsStageResult result) 
        => new (stageName, stageNumber, StageExecutingStatusType.Failed, result);
    
    public static ConsoleCommandStageState Succeeded(string stageName, int stageNumber, ConsoleCommandsStageResult result) 
        => new (stageName, stageNumber, StageExecutingStatusType.Succeeded, result);
}