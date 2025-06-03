using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Groups;

public interface IGroupRepository
{
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Dictionary<string, Group> GetDictItems();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public List<Group> GetAll();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void Save(Group test);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void SaveDictItems(Dictionary<string, Group> dictItems);

    public Group[] Get();
    public Group Get(Guid groupId);
    public Group? FindByUserId(Guid userId);
    
}