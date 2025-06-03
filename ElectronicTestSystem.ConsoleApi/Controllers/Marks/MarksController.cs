using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleApi.Contracts.Marks;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicTestSystem.ConsoleApi.Controllers.Marks;

[ApiController]
[Route("[controller]")]
public class MarksController : ControllerBase
{
    private readonly IMarkCalculatorFacade _calculatorFacade;

    public MarksController(IMarkCalculatorFacade calculatorFacade)
    {
        _calculatorFacade = calculatorFacade;
    }

    [HttpPost]
    [Route("tests/{testName}/groups/{groupName}/calculate")]
    public IActionResult Calculate(
        string testName,
        string groupName,
        CalculateMarksApiRequest request)
    {
        var testNameToTestPair = _calculatorFacade
            .GetAllTests()
            .FirstOrDefault(t => t.Key == testName);

        if (testNameToTestPair.Value == null)
        {
            return NotFound(MarkCalculatorErrors.TestNotFound);
        }
        
        var groupNameToGroupPair = _calculatorFacade
            .GetAllGroups()
            .FirstOrDefault(t => t.Key == groupName);
        
        if (groupNameToGroupPair.Value == null)
        {
            return NotFound(MarkCalculatorErrors.GroupNotFound);
        }
        
        var usersAnswers = request.UsersAnswers
            .Select(ua => new UserAnswersDto(ua.UserName, ua.Answers))
            .ToList();
        var userMarks = _calculatorFacade.CalculateMarks(
            testNameToTestPair.Value.CorrectQuestionsAnswers, usersAnswers);
        
        var userMarksResponse = new CalculateMarksApiResponse(
            userMarks.Select(um => new CalculateMarksUserMarkResponse(um.UserName, um.Mark))
                .ToList());

        return Ok(userMarksResponse);
    }
}