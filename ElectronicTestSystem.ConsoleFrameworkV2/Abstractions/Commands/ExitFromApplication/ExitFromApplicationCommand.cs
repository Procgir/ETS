using ElectronicTestsSystem.ConsoleFramework.Shared.Exceptions;

namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands.ExitFromApplication;

public class ExitFromApplicationCommand(ConsoleCommandApp app) : ConsoleCommand(app, SystemConsoleCommandsNames.ExitFromApplication)
{
    protected override void InitializeStages()
    {
        AddStage(string.Empty, () => throw new ConsoleApplicationExitException());
    }
}