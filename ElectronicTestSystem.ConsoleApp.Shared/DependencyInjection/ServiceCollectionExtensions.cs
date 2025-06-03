using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Infrastructure.DependencyInjectionExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicTestSystem.ConsoleApp.Shared.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddMarkCalculator(this IServiceCollection services)
    {
        return services.AddSingleton<IMarkCalculator, MarkCalculator>();
    }
    internal static IServiceCollection AddGroupService(this IServiceCollection services, string groupsFileName)
    {
        return services.AddGroupsFileRepository(groupsFileName)
            .AddTransient<IGroupService, GroupService>();
    }
    internal static IServiceCollection AddTestService(this IServiceCollection services, string testsFileName)
    {
        return services.AddTestsFileRepository(testsFileName)
            .AddTransient<ITestService, TestService>();
    }
    
    public static IServiceCollection AddMarkCalculatorFacade(this IServiceCollection services, string groupsFileName, string testsFileName)
    {
        return services.AddGroupService(groupsFileName)
            .AddTestService(testsFileName)
            .AddMarkCalculator()
            .AddTransient<IMarkCalculatorFacade, MarkCalculatorFacade>();
    }
    
}