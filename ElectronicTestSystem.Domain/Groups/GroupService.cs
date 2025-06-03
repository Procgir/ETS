namespace ElectronicTestSystem.Domain.Groups;

public class GroupService(IGroupRepository groupsRepository) : IGroupService
{
    public List<Group> GetAllGroups()
    {
        return groupsRepository.GetAll();;
    }

    public void SaveGroups(Dictionary<string, Group> groupsNamesUsersNames)
    {
        groupsRepository.SaveDictItems(groupsNamesUsersNames);
    }
    
    public void SaveGroup(Group group)
    {
        groupsRepository.Save(group);
    }

    public Group[] Get()
    {
        return groupsRepository.Get();
    }

    public Task<Group[]> GetByAsync(Guid[] groupsIds, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Group Get(Guid groupId)
    {
        return groupsRepository.Get(groupId);
    }

    public Task<Group?> GetByAsync(Guid groupId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Group? FindByUserId(Guid userId)
    {
        return groupsRepository.FindByUserId(userId);
    }
}