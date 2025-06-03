namespace ElectronicTestSystem.Application.Dto;

public record CreateTestingDto(Guid TestId, Guid GroupId, DateTime EndedAt, Guid AuthorId);