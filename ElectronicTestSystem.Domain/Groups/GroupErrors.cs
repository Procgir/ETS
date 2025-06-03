using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Domain.Groups;

public static class GroupErrors
{
    public static Error NotFound = new(
        "Testing.NotFound", 
        "Group not found");
}