namespace ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

internal abstract class SystemConsoleCommand(ConsoleCommandApp app, string name) : IConsoleCommand
{
    public ConsoleCommandApp App { get; } = app;
    public string Name { get; } = name;
    public ConsoleCommandType Type { get; } = ConsoleCommandType.System;

    public abstract void Execute();
}