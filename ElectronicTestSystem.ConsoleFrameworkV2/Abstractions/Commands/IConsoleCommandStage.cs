namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

public interface IConsoleCommandStage
{
    ConsoleCommandStageState Execute();
}