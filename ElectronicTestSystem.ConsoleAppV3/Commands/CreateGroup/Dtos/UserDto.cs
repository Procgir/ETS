namespace ElectronicTestSystem.ConsoleAppV3.Commands.CreateGroup.Dtos;

public record UserDto(string Name)
{
    public override string ToString()
    {
        return Name;
    }
}