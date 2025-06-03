namespace ElectronicTestsSystem.ConsoleFramework.Shared.Functions;

public delegate bool TryDoActionAndGetOutputStrings(out List<string> outputStrings);

public delegate void DoActionAndGetOutputStrings(out List<string> outputStrings);
