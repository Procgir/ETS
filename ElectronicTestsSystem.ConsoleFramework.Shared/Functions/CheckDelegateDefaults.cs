namespace ElectronicTestsSystem.ConsoleFramework.Shared.Functions;

public static class CheckDelegateDefaults
{
    public static CheckDelegate SucceededCheck = (string input, out string errorMessage) => { errorMessage = string.Empty; return true; };
}