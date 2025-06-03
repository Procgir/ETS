using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Get;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicTestSystem.ConsoleApi.Controllers.Groups;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IMarkCalculatorFacade _markCalculatorFacade;

    public GroupsController(IMarkCalculatorFacade markCalculatorFacade)
    {
        _markCalculatorFacade = markCalculatorFacade;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var groupNameToGroupResponseDict = _markCalculatorFacade.GetAllGroups()
            .Select(p => new GetGroupApiResponse(p.Key, p.Value
                .UsersNames.Select(un => new GetGroupUserApiResponse(un)).ToList()))
            .ToDictionary(r => r.Name, r => r);

        return Ok(new GetGroupsApiResponse(groupNameToGroupResponseDict));
    }

    [HttpPost]
    public IActionResult Create(CreateGroupApiRequest request)
    {
        var groupDto = new GroupDto(request.Name, 
            request.Users.Select(u => u.Name).ToList());
        
        _markCalculatorFacade.SaveGroup(groupDto);

        var group = _markCalculatorFacade.GetAllGroups()
            .FirstOrDefault(t => t.Key == request.Name).Value;;

        return Ok(new CreateGroupApiResponse(group.Name));
    }
}