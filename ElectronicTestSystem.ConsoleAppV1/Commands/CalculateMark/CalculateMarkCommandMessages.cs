namespace ElectronicTestSystem.ConsoleAppV1.Commands.CalculateMark;

public static class CalculateMarkCommandMessages
{
    public static string EnterCorrectQuestionsAnswers = "Введите номера правильных ответов на тест!";
    public static string HaveSavedCorrectQuestionsAnswersQuestion = "Обнаружен сохраненный список правильных ответов! Использовать его?";
    public static string SaveCorrectQuestionsAnswersToFile = "Сохранить правильные ответы в файл?";
    public static string ChooseTest = "Выберите тест: ";
    public static string UseSavedGroupsQuestion = "Обнаружен сохраненный список групп учеников! Использовать его?";
    public static string EnterUserName = "Введите ФИО пользователя: ";
    public static string EnterUserAnswers = "Введите список ответов пользователя: ";
    public static string EnterUserAnswerNumber = "Введите номер ответа, который указал пользователь";
    public static string IncorrectInputFormatNeedToRetryTemplate = "Неверный формат ввода: {0}. Необходимо ввести цифру. Попробуйте еще раз.";
    public static string AllAnswersEntered = "Вы ввели все ответы пользователя!";
    public static string CantCalculateMarkQuestionsCountNotEqualUserAnswersCount = "Невозможно рассчитать оценку! Количество правильных ответов не совпадает с количеством ответов пользователя.";
    public static string ContinueQuestion = "Продолжить?";
    public static string SavedGroupsToFileQuestion = "Сохранить введенных учеников в файл?";
    public static string ChooseGroup = "Выберите группу: ";
    public static string GroupExistsWithSameName = "Группа с таким названием уже существует";
    public static string EnterGroupNameForSaving = "Введите название группы для сохранения:";
    public static string EnterTestNameForSaving = "Введите название теста для сохранения: ";
    public static string TestExistsWithSameName = "Тест с таким названием уже существует ";
    public static string EnterCorrectAnswerNumber = "Введите номер правильного ответа";
}