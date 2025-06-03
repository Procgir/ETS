using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Testings;

public interface ITestingUserAnswersRepository
{
    public void Add(TestingUserAnswers testingUserAnswers);
    public void Update(TestingUserAnswers testingUserAnswers);
    public TestingUserAnswers GetBy(Guid testingId, Guid userId);
    public Task<TestingUserAnswers> GetByAsync(Guid testingId, Guid userId, CancellationToken cancellationToken);
    public Task<TestingUserAnswers[]> GetByAsync(Guid testingId, CancellationToken cancellationToken);
    
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Guid Create(TestingUserAnswers testingUserAnswers);
}