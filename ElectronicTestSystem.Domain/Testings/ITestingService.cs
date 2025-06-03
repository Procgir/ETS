using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Domain.Testings;

public interface ITestingService
{
    public Testing Get(Guid testingId);
    public Task<Testing?> GetAsync(Guid testingId, CancellationToken cancellationToken);
    public PaginatedResult<Testing> Get(GetOptions getOptions);
    public void Create(Testing testing);
    public void Update(Testing testing);
    public Testing[]? Find(string query);
    public void Delete(Guid testingId);
}