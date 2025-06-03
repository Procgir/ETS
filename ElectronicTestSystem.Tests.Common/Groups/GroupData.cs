using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Tests.Common.Groups;

public static class GroupData
{
    public static Group Default => Group.Create("Test Group", new List<User>());
}