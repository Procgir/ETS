using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Domain.Tests.Events;

public sealed record class TestCreatedDomainEvent(Guid TestId) : IDomainEvent;
