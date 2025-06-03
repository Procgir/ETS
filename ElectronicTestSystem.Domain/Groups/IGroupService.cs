using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Groups;

public interface IGroupService
{
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public List<Group> GetAllGroups();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]

    public void SaveGroups(Dictionary<string, Group> groupsNamesUsersNames);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void SaveGroup(Group group);

    public Group[] Get();
    public Task<Group[]> GetByAsync(Guid[] groupsIds, CancellationToken cancellationToken);
    public Group Get(Guid groupId);
    public Task<Group?> GetByAsync(Guid groupId, CancellationToken cancellationToken);
    public Group? FindByUserId(Guid userId);
}