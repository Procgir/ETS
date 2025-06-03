namespace ElectronicTestSystem.Domain.Testings;

public class TestingUserAnswerService : ITestingUserAnswerService
{
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepository;

    public TestingUserAnswerService(ITestingUserAnswersRepository testingUserAnswersRepository)
    {
        _testingUserAnswersRepository = testingUserAnswersRepository;
    }
    
    public Guid Create(TestingUserAnswers testingUserAnswers)
    {
        return _testingUserAnswersRepository.Create(testingUserAnswers);
    }

    public void Update(TestingUserAnswers testingUserAnswers)
    {
        _testingUserAnswersRepository.Update(testingUserAnswers);
    }

    public Task<TestingUserAnswers> GetByAsync(Guid testingId, Guid userId, CancellationToken cancellationToken)
    {
        return _testingUserAnswersRepository.GetByAsync(testingId, userId, cancellationToken);
    }
}