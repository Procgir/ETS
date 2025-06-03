using ElectronicTestsSystem.ConsoleFramework.Shared.Components;

namespace ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;

public static class ConsoleInputsStringExtensions
{
    public static YesNoAnswerType ToYesNoAnswerType(this string input)
    {
        if (input.ToLower() == InputCommands.Yes)
        {
            return YesNoAnswerType.Yes;
        }

        if (input.ToLower() == InputCommands.No)
        {
            return YesNoAnswerType.No;
        }
        
        return YesNoAnswerType.Unknown;
    }
}