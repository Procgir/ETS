namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

internal class ConsoleCommandStage(ConsoleCommand command, string name, int number, ConsoleCommandStageAction stageAction) : IConsoleCommandStage
{
    public ConsoleCommand Command { get; internal set; } = command;
    public string Name { get; } = name;
    public int Number { get; } = number;
    public ConsoleCommandStageAction StageAction { get; } = stageAction;
    public bool IsFirst { get; internal set; }
    public bool IsLast { get; internal set; }

    public ConsoleCommandStageState Execute() 
    {
        var result = StageAction.Invoke();
        if (!result.IsSuccess)
        {
            var stageResult = new ConsoleCommandsStageResult(
                new List<string>(0), 
                new List<string> { result.ErrorMessage! });

            return ConsoleCommandStageState.Failed(Name, Number, stageResult);
        }
        
        return ConsoleCommandStageState.Succeeded(Name, Number, result.Value!);
    }
}