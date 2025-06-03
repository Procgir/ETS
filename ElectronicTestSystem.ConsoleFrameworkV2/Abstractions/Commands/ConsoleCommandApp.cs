using ElectronicTestsSystem.ConsoleFramework.Shared.Components;
using ElectronicTestsSystem.ConsoleFramework.Shared.Exceptions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Functions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Messages;
using ElectronicTestsSystem.ConsoleFramework.Shared.Settings;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

public abstract class ConsoleCommandApp
{
    private readonly List<IConsoleCommand> _customConsoleCommands = new List<IConsoleCommand>();

    public ConsoleCommandApp()
    {
    }

    protected abstract void AddCommands();

    protected void AddCommand(IConsoleCommand consoleCommand)
    {
        _customConsoleCommands.Add(consoleCommand);
    }

    public void Run(string[] args)
    {
        Initialize();

        do
        {
            try
            {
                RunInner();
            }
            catch (ConsoleApplicationExitException ex)
            {
                break;
            }
        } while (true);
    }

    private void RunInner()
    {
        (int commandNumber, string _) = ChooseCommand();

        var command = _customConsoleCommands[commandNumber - 1];
        
        ExecuteCommand(command);
        
        command.Reset();

        if (ReadUserYesOrNoAnswer(ConsoleMessages.ReturnToChooseCommandOrExit).IsNo())
        {
            throw new ConsoleApplicationExitException();
        }

        ClearConsole();
    }

    private void ExecuteCommand(IConsoleCommand command)
    {
        Console.WriteLine(ConsoleMessages.ChosenCommandTemplate, command.Name);
        
        foreach (var stageState in command.Execute())
        {
            var result = stageState.Result;
            var renderInfo = new RenderInfo(0, stageState.BeginTopPosition, 0, stageState.EndTopPosition, 
                result.InputStrings,
                result.OutputStrings);

            if (stageState.Status == StageExecutingStatusType.Failed)
            {
                ClearAndShow(renderInfo, true, false, true);
                continue;
            }
            
            if (stageState is {IsFirst: true} or 
                {IsFirst: true, IsLast: true})
            {
                ClearAndShow(renderInfo, true, true, true);
                continue;
            }

            ClearAndShow(renderInfo, true, true, true);
        }
    }

    private (int optionNumber, string optionText) ChooseCommand()
    {
        return ChooseFromListOptions(
            ConsoleMessages.EnterCommandNumberToExecute,
            GetListCommands().ToList());
    }
    
    private static void ClearConsole()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }

    private void ClearAndShow(RenderInfo renderInfo, bool beginWrap, bool needMiddleDelimeter, bool endWrap)
    {
        ClearPartOfConsole(renderInfo.BeginLeft, renderInfo.BeginTop,
            Console.WindowWidth, renderInfo.EndTop - renderInfo.BeginTop);
        
        Console.SetCursorPosition(renderInfo.BeginLeft, renderInfo.BeginTop);

        ShowWrapped(renderInfo.InputStrings, renderInfo.OutputStrings, beginWrap, needMiddleDelimeter, endWrap);
    }
    
    public void ShowWrapped(List<string> inputStrings, List<string> outputStrings, bool needBeginWrap, bool needMiddleDelimeter, bool needEndWrap)
    {
        if (needBeginWrap)
        {
            WrapperLine(ConsoleAppSettings.WrapperBorderSymbol);
        }
        
        foreach (var @string in inputStrings)
        {
            Console.WriteLine(@string);
        }

        if (inputStrings.Count == 0 && needMiddleDelimeter)
        {
            Console.WriteLine();
        }

        if (needMiddleDelimeter)
        {
            WrapperLine(ConsoleAppSettings.WrapperDelimeterSymbol);
        }
        
        foreach (var @string in outputStrings)
        {
            Console.WriteLine(@string);
        }
        
        if (outputStrings.Count == 0)
        {
            Console.WriteLine();
        }
        
        if (needEndWrap)
        {
            WrapperLine(ConsoleAppSettings.WrapperBorderSymbol);
        }
    }

    private void WrapperLine(char wrapperSymbol)
    {
        Console.Write(new string(wrapperSymbol, ConsoleAppSettings.WrapperSymbolWidth));
        Console.WriteLine();
    }
    
    private void ClearPartOfConsole(int startX, int startY, int width, int height)
    {
        Console.SetCursorPosition(startX, startY);

        for (int y = 0; y < height; y++)
        {
            Console.Write(new string(' ', width));
            if (y < height - 1) 
            {
                Console.SetCursorPosition(startX, startY + y + 1);
            }
        }
    }

    public bool TryReadInputListUntilStopOrEnd<TParsedInput>(string beginMessage,
        IsEndDelegate isEndCheck,
        string allInputEnteredTemplate,
        TryParseDelegate<TParsedInput> tryParseFunc,
        string cantParseInputMessageTemplate,
        string hasStoppedMessage,
        out List<TParsedInput> inputList)
    {
        inputList = new List<TParsedInput>();
        while (true)
        {
            Console.Write(ConsoleMessages.ForTerminateEnterStopAndEnterTemplate, beginMessage);

            string input = Console.ReadLine();

            if (input.Length > 1)
            {
                if (input == InputCommands.Stop)
                {
                    if (string.IsNullOrWhiteSpace(hasStoppedMessage))
                    {
                        Console.WriteLine(hasStoppedMessage);
                    }

                    ;
                    return false;
                }

                Console.WriteLine(ConsoleMessages.UnknownCommandTryAgainTemplate, input);
                continue;
            }

            if (!tryParseFunc.Invoke(input, out TParsedInput parsedInput))
            {
                if (string.IsNullOrWhiteSpace(cantParseInputMessageTemplate))
                {
                    Console.WriteLine(cantParseInputMessageTemplate, input);
                }

                continue;
            }

            inputList.Add(parsedInput);

            if (isEndCheck.Invoke(input, inputList.Count))
            {
                Console.WriteLine(allInputEnteredTemplate, input);
                return true;
            }
        }
    }

    public YesNoAnswerType ReadUserYesOrNoAnswer(string message)
    {
        Console.WriteLine(ConsoleMessages.MessageWithYesOrNoTemplate, message);

        while (true)
        {
            string input = Console.ReadLine();
            YesNoAnswerType answer = input.ToYesNoAnswerType();

            if (answer.IsUnknown())
            {
                Console.WriteLine(ConsoleMessages.UnknownCommandTryAgainTemplate, input);
                continue;
            }

            return answer;
        }
    }

    public (int optionNumber, string optionText) ChooseFromListOptions(string message, List<string> options)
    {
        ShowOptionsList(message, options);

        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out var chosenOptionNumber) ||
                chosenOptionNumber < 1 || options.Count < chosenOptionNumber)
            {
                Console.WriteLine(ConsoleMessages.IncorrectNumberInputTryAgain);
                continue;
            }

            return (chosenOptionNumber, options[chosenOptionNumber - 1]);
        }
    }

    private static void ShowOptionsList(string message, List<string> options)
    {
        Console.WriteLine(message);

        foreach (var optionWithIndex in options.WithIndex())
        {
            Console.WriteLine($"{optionWithIndex.Index + 1}. {optionWithIndex.Item}");
        }
    }

    public string ReadInputUntilValid(string message, CheckDelegate checkFunc)
    {
        while (true)
        {
            Console.WriteLine(message);

            string input = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine(ConsoleMessages.EnteredInputMustNotBeEmpty);
                continue;
            }

            if (!checkFunc(input, out var errorMessage))
            {
                Console.WriteLine(ConsoleMessages.MessageWithTryAgainTemplate, errorMessage);
                continue;
            }

            return input;
        }
    }

    public string ReadInputUntilValidWithoutTitle(CheckDelegate checkFunc)
    {
        while (true)
        {
            string input = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine(ConsoleMessages.EnteredInputMustNotBeEmpty);
                continue;
            }

            if (!checkFunc(input, out var errorMessage))
            {
                Console.WriteLine(ConsoleMessages.MessageWithTryAgainTemplate, errorMessage);
                continue;
            }

            return input;
        }
    }
    
    private IEnumerable<string> GetListCommands()
    {
        foreach (var command in _customConsoleCommands)
        {
            yield return command.Name;
        }
    }

    private void Initialize()
    {
        AddCommands();
    }
}