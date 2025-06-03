using System.Reflection;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ElectronicTestSystem.ArchitecturalTests.Infrastructure;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(IBaseCommand).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
