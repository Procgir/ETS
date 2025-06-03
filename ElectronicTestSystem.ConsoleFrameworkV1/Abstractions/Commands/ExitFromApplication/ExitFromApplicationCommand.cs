using ElectronicTestsSystem.ConsoleFramework.Shared.Exceptions;

namespace ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands.ExitFromApplication;

internal class ExitFromApplicationCommand(ConsoleCommandApp app, string name) : SystemConsoleCommand(app, name)
{
    public override void Execute()
    {
        throw new ConsoleApplicationExitException();
    }
}