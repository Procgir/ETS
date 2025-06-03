using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Infrastructure.Groups.Converters;
using ElectronicTestSystem.Infrastructure.Groups.Repositories;
using ElectronicTestSystem.Infrastructure.Tests.Converters;
using ElectronicTestSystem.Infrastructure.Tests.Repositories;
using ElectronicTestSystem.SDK.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicTestSystem.Infrastructure.DependencyInjectionExtensions;

[Obsolete("Use DependencyInjection instead")]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGroupsFileRepository(this IServiceCollection services, 
        string groupsFileName)
    {
        return services.AddSingleton<Converter<Group>, GroupConverter>()
            .AddTransient<IGroupRepository, GroupsFileRepository>(sp =>
                new GroupsFileRepository(FileRepositoriesFileNames.Groups, sp.GetService<Converter<Group>>()));
    }
    
    public static IServiceCollection AddTestsFileRepository(this IServiceCollection services, 
        string groupsFileName)
    {
        return services.AddSingleton<Converter<Test>, TestConverter>()
            .AddTransient<ITestRepository, TestsFileRepository>(sp =>
                new TestsFileRepository(FileRepositoriesFileNames.Tests, sp.GetService<Converter<Test>>()));
    }
    
}