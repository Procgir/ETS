using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Domain.Testings;

public class TestingService : ITestingService
{
    private readonly ITestingRepository _testingRepository;

    public TestingService(ITestingRepository testingRepository)
    {
        _testingRepository = testingRepository;
    }
    
    public Testing Get(Guid testingId)
    {
        return _testingRepository.Get(testingId);
    }

    public Task<Testing?> GetAsync(Guid testingId, CancellationToken cancellationToken)
    {
        return _testingRepository.GetAsync(testingId, cancellationToken);
    }

    public PaginatedResult<Testing> Get(GetOptions getOptions)
    {
        return _testingRepository.Get(getOptions);
    }

    public void Create(Testing testing)
    { 
        _testingRepository.Create(testing);
    }

    public void Update(Testing testing)
    {
        _testingRepository.Update(testing);
    }

    public Testing[]? Find(string query)
    {
        return _testingRepository.Find(query);
    }

    public void Delete(Guid testingId)
    {
        _testingRepository.Delete(testingId);
    }
}