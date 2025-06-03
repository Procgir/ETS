namespace ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

public interface IConsoleCommand
{
    public string Name { get; }
    public ConsoleCommandType Type { get; }
    public void Execute();
}