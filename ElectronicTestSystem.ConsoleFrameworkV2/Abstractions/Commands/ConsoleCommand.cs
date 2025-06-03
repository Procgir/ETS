using ElectronicTestsSystem.ConsoleFramework.Shared.Messages;

namespace ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;

public abstract class ConsoleCommand(ConsoleCommandApp app, string name) : IConsoleCommand
{
    protected ConsoleCommandApp App { get; } = app;
    public string Name { get; } = name;
    public ConsoleCommandStageState? Current { get; private set; }
    internal List<ConsoleCommandStage> Stages { get; } = new List<ConsoleCommandStage>();
    

    protected void AddStage(string name, ConsoleCommandStageAction stageAction)
    {
        var currentStage = new ConsoleCommandStage(this, name, Stages.Count + 1, stageAction);
            
        Stages.Add(currentStage);

        if (Stages.Count == 1)
        {
            currentStage.IsFirst = true;
            currentStage.IsLast = true;
        }
        else
        {
            var prevStage = Stages[^2];
            prevStage.IsLast = false;
            currentStage.IsLast = true;
        }
    }

    protected abstract void InitializeStages();

    public IEnumerable<ConsoleCommandStageState> Execute()
    {
        if (Stages.Count == 0)
        {
            InitializeStages();
        }
        
        foreach (var stage in Stages)
        {
            if (Current is {Status: StageExecutingStatusType.Failed})
            {
                yield break;
            }
            
            Console.WriteLine(ConsoleMessages.ExecutingCommandStageTemplate, stage.Number, stage.Name);

            int topBeginPosition = Console.CursorTop;
            Current = stage.Execute();
            int topEndPosition = Console.CursorTop;

            Current.BeginTopPosition = topBeginPosition;
            Current.EndTopPosition = topEndPosition;
            Current.IsFirst = stage.IsFirst;
            Current.IsLast = stage.IsLast;
            
            yield return Current;
        }
    }

    public void Reset()
    {
        Current = null;
    }
}