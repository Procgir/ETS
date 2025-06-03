using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Domain.Tests;

public class TestService : ITestService
{
    private readonly ITestRepository _testsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TestService(ITestRepository testsRepository, IUnitOfWork unitOfWork)
    {
        _testsRepository = testsRepository;
        _unitOfWork = unitOfWork;
    }
    
    public Dictionary<string, Test> GetAllTests()
    {
        return _testsRepository.GetDictItems();
    }
    
    public List<Test> GetAll()
    {
        return _testsRepository.GetAll();
    }

    public void Create(Test test)
    {
        _testsRepository.Create(test);
    }
    
    public async Task CreateAsync(Test test, CancellationToken cancellationToken)
    {
        _testsRepository.Add(test);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Test Get(Guid testId)
    {
        return _testsRepository.Get(testId);
    }

    public void Delete(Guid testId)
    {
        _testsRepository.Delete(testId);
    }

    public async Task DeleteAsync(Guid testId, CancellationToken cancellationToken)
    {
        await _testsRepository.RemoveAsync(testId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public void Update(Test test)
    {
        _testsRepository.Update(test);
    }

    public async Task UpdateAsync(Test test, CancellationToken cancellationToken)
    {
        await _testsRepository.UpdateAsync(test);

        await _unitOfWork.SaveChangesAsync();
    }

    public TestSubject CreateSubject(string? subject)
    {
        return subject == null
            ? new TestSubject(string.Empty)
            : new TestSubject(subject);
    }

    public TestTheme CreateTheme(string? theme)
    {
        return theme == null
            ? new TestTheme(string.Empty)
            : new TestTheme(theme);
    }

    public TestQuestion CreateQuestion(Guid testId, string body, string[] answersOptions, int trueAnswerNumber)
    {
        return TestQuestion.Create(testId, body, answersOptions, trueAnswerNumber);
    }

    public PaginatedResult<Test> Get(GetOptions getOptions)
    {
        return _testsRepository.Get(getOptions);
    }

    public void SaveTests(Dictionary<string, Test> tests)
    {
        _testsRepository.SaveDictItems(tests);
    }
    
    public void SaveTest(Test test)
    {
        _testsRepository.Save(test);
    }
}