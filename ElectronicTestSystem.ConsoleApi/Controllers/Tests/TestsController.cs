using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Get;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicTestSystem.ConsoleApi.Controllers.Tests;

[ApiController]
[Route("[controller]")]
public class TestsController : ControllerBase
{
    private readonly IMarkCalculatorFacade _markCalculatorFacade;

    public TestsController(IMarkCalculatorFacade markCalculatorFacade)
    {
        _markCalculatorFacade = markCalculatorFacade;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        var testNameToTestResponseDict = _markCalculatorFacade.GetAllTests()
            .Select(p => new GetTestApiResponse(p.Key, p.Value.CorrectQuestionsAnswers))
            .ToDictionary(r => r.Name, r => r);
        
        return Ok(new GetTestsApiResponse(testNameToTestResponseDict));
    }
    
    [HttpPost]
    public IActionResult Create(CreateTestApiRequest request)
    {
        var testDto = new TestDto(request.Name, request.CorrectQuestionsAnswers);
        
        _markCalculatorFacade.SaveTest(testDto);
        
        var test = _markCalculatorFacade.GetAllTests()
            .FirstOrDefault(t => t.Key == request.Name).Value;;

        return Ok(new CreateTestApiResponse(test.Name));
    }
}