namespace ElectronicTestSystem.ConsoleAppV2.Commands.CreateTest;

public static class CreateTestMessages
{
    public static string IncorrectInputFormatNeedToRetryTemplate = "Неверный формат ввода: {0}. Необходимо ввести цифру. Попробуйте еще раз.";
    public static string EnterTestNameForSaving = "Введите название теста для сохранения:";
    public static string TestExistsWithSameName = "Тест с таким названием уже существует ";
    public static string EnterCorrectQuestionsAnswers = "Введите номера правильных ответов на тест!";
    public static string EnterCorrectAnswerNumber = "Введите номер правильного ответа";
    
    public static string SingleStageName = "Создание теста";
    public static string TestNameInputTemplate = "Название теста: {0}";
    public static string CorrectQuestionAnswersTitleInput = "Правильные ответы на вопросы теста";

    public static string CorrectQuestionAnswerInputTemplate = "{0}. {1}";
    
    public static string TestSavedSuccessfullyOutput = "Тест успешно сохранен";

}