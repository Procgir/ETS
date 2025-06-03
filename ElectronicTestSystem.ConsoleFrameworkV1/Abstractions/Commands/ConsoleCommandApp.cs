using ElectronicTestsSystem.ConsoleFramework.Shared.Components;
using ElectronicTestsSystem.ConsoleFramework.Shared.Exceptions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Functions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Messages;
using ElectronicTestsSystem.ConsoleFramework.Shared.Settings;
using ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands.ExitFromApplication;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

public abstract class ConsoleCommandApp
{
    private readonly List<IConsoleCommand> _customConsoleCommands = new List<IConsoleCommand>();
    private int LastNumberCommand => _customConsoleCommands.Count;
    private int NextNumberCommand => LastNumberCommand + 1;

    public ConsoleCommandApp()
    {
    }

    protected abstract void AddCommands();

    internal void AddSystemCommands()
    {
        AddCommand(new ExitFromApplicationCommand(this, SystemConsoleCommandsNames.ExitFromApplication));
    }

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

        ExecuteCommand(commandNumber);

        if (ReadUserYesOrNoAnswer(ConsoleMessages.ReturnToChooseCommandOrExit).IsNo())
        {
            throw new ConsoleApplicationExitException();
        }

        ClearConsole();
    }

    private void ExecuteCommand(int commandNumber)
    {
        _customConsoleCommands[commandNumber - 1].Execute();
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

    public bool TryDoActionWithRerenderWithOutputStrings(TryDoActionAndGetOutputStrings tryAction, bool beginWrap, bool endWrap)
    {
        var isSucceeded = WrapWithBetweenAreaPosition(tryAction, out var renderInfo);

        ClearAndShow(renderInfo, beginWrap,endWrap);
        
        return isSucceeded;
    }

    private void ClearAndShow(RenderInfo renderInfo, bool beginWrap, bool endWrap)
    {
        ClearPartOfConsole(renderInfo.BeginLeft, renderInfo.BeginTop,
            Console.WindowWidth, renderInfo.EndTop - renderInfo.BeginTop);
        
        Console.SetCursorPosition(renderInfo.BeginLeft, renderInfo.BeginTop);

        ShowWrapped(renderInfo.OutputStrings, beginWrap, endWrap);
    }

    public void ShowWrapped(List<string> stringsForOutput, bool needBeginWrap, bool needEndWrap)
    {
        if (needBeginWrap)
        {
            WrapperLine();
        }
        
        foreach (var @string in stringsForOutput)
        {
            Console.WriteLine(@string);
        }
        
        if (needEndWrap)
        {
            WrapperLine();
        }
    }

    private void WrapperLine()
    {
        Console.Write(new string(ConsoleAppSettings.WrapperBorderSymbol, ConsoleAppSettings.WrapperSymbolWidth));
        Console.WriteLine();
    }

    private bool WrapWithBetweenAreaPosition(TryDoActionAndGetOutputStrings tryAction, 
           out RenderInfo renderInfo)
    {
        var currentCursorPosition = Console.GetCursorPosition();

        var isSucceeded = tryAction.Invoke(out var stringsForOutput);
        
        var afterActionCursorPosition = Console.GetCursorPosition();

        renderInfo = new RenderInfo(currentCursorPosition.Left, currentCursorPosition.Top,
            afterActionCursorPosition.Left, afterActionCursorPosition.Top, 
            new List<string>(0), stringsForOutput);
        
        return isSucceeded;
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
        AddSystemCommands();
    }
}