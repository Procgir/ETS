using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleAppV2.Commands.CalculateMarkLinear;
using ElectronicTestSystem.ConsoleAppV2.Commands.CreateGroup;
using ElectronicTestSystem.ConsoleAppV2.Commands.CreateTest;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands.ExitFromApplication;

namespace ElectronicTestSystem.ConsoleAppV2;

public class CalculateMarkApp(IMarkCalculatorFacade markCalculatorFacade) : ConsoleCommandApp
{
    protected override void AddCommands()
    {
        AddCommand(new CalculateMarkLinearCommand(this, markCalculatorFacade));
        AddCommand(new CreateGroupCommand(this, markCalculatorFacade));
        AddCommand(new CreateTestCommand(this, markCalculatorFacade));
        AddCommand(new ExitFromApplicationCommand(this));
    }
}