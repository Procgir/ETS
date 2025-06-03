namespace ElectronicTestSystem.WebApi.Controllers.Testings;

public sealed record AssignTestingRequest(Guid TestId, Guid GroupId, DateTime EndDate);