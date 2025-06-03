namespace ElectronicTestSystem.Domain.Testings;

public interface ITestingUserAnswerService
{
    public Guid Create(TestingUserAnswers testingUserAnswers);
    public void Update(TestingUserAnswers testingUserAnswers);
    public Task<TestingUserAnswers> GetByAsync(Guid testingId, Guid userId, CancellationToken cancellationToken); 
}