namespace ElectronicTestSystem.WebApi.Controllers.Tests;

public sealed record UpdateTestRequest(
    string Subject,
    string Theme,
    UpdateTestRequestQuestion[] Questions);