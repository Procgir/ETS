using ElectronicTestSystem.SDK.Data;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Tests;

public interface ITestService
{
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Dictionary<string, Test> GetAllTests();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void SaveTests(Dictionary<string, Test> tests);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void SaveTest(Test test);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public List<Test> GetAll();
    
    public void Create(Test test);
    public Task CreateAsync(Test test, CancellationToken cancellationToken);
    public Test Get(Guid testId);
    public PaginatedResult<Test> Get(GetOptions getOptions);
    public void Delete(Guid testId);
    public Task DeleteAsync(Guid testId, CancellationToken cancellationToken);
    public void Update(Test test);
    public Task UpdateAsync(Test test, CancellationToken cancellationToken);

    public TestSubject CreateSubject(string? subject);
    public TestTheme CreateTheme(string? theme);
    public TestQuestion CreateQuestion(Guid testId, string body, string[] answersOptions, int trueAnswerNumber);
}