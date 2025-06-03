using ElectronicTestSystem.Application.Testings.GetTesting;
using ElectronicTestSystem.Domain.Testings;

namespace ElectronicTestSystem.Application.Testings.GetTestings;

public record TestingsItemResponse(
    Guid TestingId,
    TestingsItemResponseStatus ResponseStatus,
    string GroupName,
    string TestName,
    DateTime CreatedAt,
    DateTime EndedAt,
    int DoneCount,
    int AllCount);
    
public enum TestingsItemResponseStatus
{
    Unknown = 0,
    Active = 1,
    Finished = 2
}