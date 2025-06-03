using ElectronicTestsSystem.ConsoleFramework.Shared.Components;

namespace ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;

public static  class YesNoAnswerExtension
{
    public static bool IsYes(this YesNoAnswerType answerType) => answerType == YesNoAnswerType.Yes;
    public static bool IsNo(this YesNoAnswerType answerType) => answerType == YesNoAnswerType.No;
    public static bool IsUnknown(this YesNoAnswerType answerType) => answerType == YesNoAnswerType.Unknown;
}