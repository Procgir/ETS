using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Infrastructure.Abstractions;
using ElectronicTestSystem.SDK.Converters;

namespace ElectronicTestSystem.Infrastructure.Groups.Repositories;

public class GroupsFileRepository(string fileName, Converter<Group> converter)
    : FileRepository<Group>(fileName, converter), IGroupRepository
{
    public Group[] Get()
    {
        throw new NotImplementedException();
    }

    public Group Get(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Group? FindByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }
}