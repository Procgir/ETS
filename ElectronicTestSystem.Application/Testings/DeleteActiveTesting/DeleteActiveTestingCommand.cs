using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Testings.DeleteActiveTesting;

public record DeleteActiveTestingCommand(Guid TestingId) : ICommand<TestingsItemResponse>;