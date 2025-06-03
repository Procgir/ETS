namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

public interface IConsoleCommand
{
    public string Name { get; }
    public ConsoleCommandStageState? Current { get; }
    internal IEnumerable<ConsoleCommandStageState> Execute();
    internal void Reset();
}