using ElectronicTestSystem.Application.Dto;

namespace ElectronicTestSystem.Application.Tests.FindTest;

public sealed class TestsResponse
{
    public TestShortResponse[] Tests { get; internal set; }
}