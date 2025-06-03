using ElectronicTestSystem.ConsoleApi.Clients.Groups;
using ElectronicTestSystem.ConsoleApi.Clients.Marks;
using ElectronicTestSystem.ConsoleApi.Clients.Tests;
using ElectronicTestSystem.ConsoleAppV3.Commands.CalculateMarkLinear;
using ElectronicTestSystem.ConsoleAppV3.Commands.CreateGroup;
using ElectronicTestSystem.ConsoleAppV3.Commands.CreateTest;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands.ExitFromApplication;

namespace ElectronicTestSystem.ConsoleAppV3;

public class CalculateMarkApp(IMarksHttpClientApi marksHttpClientApi,
    ITestsHttpClientApi testsHttpClientApi,
    IGroupsHttpClientApi groupsHttpClientApi) : ConsoleCommandApp
{
    protected override void AddCommands()
    {
        AddCommand(new CalculateMarkLinearCommand(this, groupsHttpClientApi, testsHttpClientApi, marksHttpClientApi));
        AddCommand(new CreateGroupCommand(this, groupsHttpClientApi));
        AddCommand(new CreateTestCommand(this, testsHttpClientApi));
        AddCommand(new ExitFromApplicationCommand(this));
    }
}