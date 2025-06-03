namespace ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

public abstract class ConsoleCommand(ConsoleCommandApp app, string name) : IConsoleCommand
{
    protected ConsoleCommandApp App { get; } = app;
    public string Name { get; } = name;
    public ConsoleCommandType Type { get; } = ConsoleCommandType.User;
    public abstract void Execute();
}