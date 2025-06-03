namespace ElectronicTestSystem.ConsoleAppV3.Commands.CreateGroup.Dtos;

public record GroupDto(string Name, List<string> UsersNames)
{
    public static GroupDto Empty() => new GroupDto(string.Empty, new List<string>(0));
}