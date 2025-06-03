namespace ElectronicTestSystem.Application.Dto;

public sealed class TestShortResponse
{
    public Guid Id { get; internal set; }
    public string Subject { get; internal set; }
    public string Theme { get; internal set; }
    public DateTime CreatedAt { get; internal set; }

    public TestShortResponse()
    {
        
    }
    
    public TestShortResponse(Guid id, string subject, string theme, DateTime createdAt)
    {
        Id = id;
        Subject = subject;
        Theme = theme;
        CreatedAt = createdAt;
    }
}
    