namespace ElectronicTestSystem.WebApi.Controllers.Tests;

public sealed record CreateTestRequest(string Subject,
    string Theme,
    CreateTestRequestQuestion[] Questions);