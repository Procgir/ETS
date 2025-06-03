namespace ElectronicTestSystem.ConsoleAppV2.Commands.CalculateMarkLinear;

public static class CalculateMarkLinearCommandMessages
{
    public static string EnterCorrectQuestionsAnswers = "Введите номера правильных ответов на тест!";
    public static string ChooseTest = "Выберите тест: ";
    public static string EnterUserAnswers = "Введите список ответов пользователя: ";
    public static string EnterUserAnswerNumber = "Введите номер ответа, который указал пользователь";
    public static string IncorrectInputFormatNeedToRetryTemplate = "Неверный формат ввода: {0}. Необходимо ввести цифру. Попробуйте еще раз.";
    public static string AllAnswersEntered = "Вы ввели все ответы пользователя!";
    public static string CantCalculateMarkQuestionsCountNotEqualUserAnswersCount = "Невозможно рассчитать оценку! Количество правильных ответов не совпадает с количеством ответов пользователя.";
    public static string ChooseGroup = "Выберите группу: ";
    public static string SelectedTest = "Выбран тест: ";
    public static string SelectedGroup = "Выбрана группа: ";
    public static string EnteredAnswers = "Введеные ответы пользователя: ";
    
    public static string SelectTestFirstStageName = "Выбор теста";
    public static string SelectGroupSecondStageName = "Выбор группы";
    public static string EnterUsersAnswersThirdStageName = "Ввод ответов учеников";
    public static string CalculateMarkForGroupFourthStageName = "Расчет оценок по группе учеников: ";
    
    public static string SelectedTestNameInputTemplate = "Выбран тест: {0}";
    public static string TestCorrectQuestionsAnswersTitleInput = "Правильные ответы на вопросы теста: ";
    public static string TestCorrectQuestionsAnswerInputTemplate = "{0}. {1}";
    public static string TestChosenSuccessfully = "Тест успешно выбран! ";
    
    public static string SelectedGroupNameInputTemplate = "Выбрана группа: {0}";
    public static string UsersGroupTitleInput = "Ученики группы: ";
    public static string UsersGroupInputTemplate = "{0}. {1}";
    public static string GroupChosenSuccessfully = "Группа успешно выбрана! ";
    
    public static string EnteredUsersAnswersTitleInput = "Введенные ответы учеников: ";
    public static string UserAnswersInputTemplate = "{0}. {1}";
    public static string UsersAnswersEnteredSuccessfully = "Ответы учеников успешно введены!";
    
    public static string UserMarkCalculatedTitleInput = "Рассчитанные оценки пользователей: ";
    public static string UserMarkInputTemplate = "{0}. {1} - {2}";
    public static string UserMarkCalculatedSuccessfully = "Оценки успешно рассчитаны! ";
}