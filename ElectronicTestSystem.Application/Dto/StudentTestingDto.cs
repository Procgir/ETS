namespace ElectronicTestSystem.Application.Dto;

public class StudentTestingDto
{
    public Guid TestingId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Score { get; set; }
}
