using ElectronicTestSystem.Application.Dto;

namespace ElectronicTestSystem.Application.Testings;
//TODO: добавить валидацию и изменить результат
public interface ITestingFacade
{
    public Guid CreateTesting(CreateTestingDto createTestingDto);
    public void UpdateTesting(Guid testingId, UpdateTestingDto updateTestingDto);
    public Guid CreateTestingAnswers(Guid testingId, Guid userId, CreateTestingAnswersDto createTestingAnswersDto);

    public void UpdateTestingAnswers(Guid testingId, Guid userId, UpdateTestingAnswersDto updateTestingAnswersDto);
    //TODO: добавить остальные методы при реализации Presentation слоя
}

