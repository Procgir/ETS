using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.Domain.Testings;

public class Testing : Entity
{
    public DateTime CreatedAt { get; }
    public DateTime EndedAt { get; private set; }
    public Guid TestId { get; }
    public Guid GroupId { get; }
    public Guid AuthorId { get; }
    
    public Test Test { get; }
    
    private Testing(){}

    public TestingStatus StatusAt(DateTime now)
    {
        if (now < EndedAt)
        {
            return TestingStatus.Active;
        }

        return TestingStatus.Finished;  
    }

    public Result ChangeTesting(DateTime endedAt)
    {
        if (endedAt < CreatedAt)
        {
            return Result.Failure(TestingErrors.NotChanged);
        }
        
        EndedAt = endedAt;
        
        return Result.Success();
    } 
    
    internal Testing(Guid id, 
        DateTime createdAt, 
        DateTime endedAt,
        Guid testId,
        Guid groupId,
        Guid authorId) : base(id)
    {
        CreatedAt = createdAt;
        EndedAt = endedAt;
        TestId = testId;
        GroupId = groupId;
        AuthorId = authorId;
    }

    public static Testing Create(
        Guid testId,
        Guid groupId,
        DateTime endedAt,
        Guid authorId) 
        => new(Guid.NewGuid(), DateTime.Now, endedAt, testId, groupId, authorId);
}