using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleAppV1.Commands.CalculateMarkLinear;
using ElectronicTestSystem.ConsoleAppV1.Commands.CreateGroup;
using ElectronicTestSystem.ConsoleAppV1.Commands.CreateTest;
using ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

namespace ElectronicTestSystem.ConsoleAppV1;

public class CalculateMarkApp(IMarkCalculatorFacade markCalculatorFacade) : ConsoleCommandApp
{
    protected override void AddCommands()
    {
        AddCommand(new CalculateMarkLinearCommand(this, markCalculatorFacade));
        AddCommand(new CreateGroupCommand(this, markCalculatorFacade));
        AddCommand(new CreateTestCommand(this, markCalculatorFacade));
    }
}