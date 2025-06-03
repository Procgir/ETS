using ElectronicTestSystem.Application.Abstractions.Authentication;
using ElectronicTestSystem.Application.Tests.CreateTest;
using ElectronicTestSystem.Application.Tests.DeleteTest;
using ElectronicTestSystem.Application.Tests.FindTest;
using ElectronicTestSystem.Application.Tests.GetTest;
using ElectronicTestSystem.Application.Tests.GetTests;
using ElectronicTestSystem.Application.Tests.UpdateTest;
using ElectronicTestSystem.Domain.Tests.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicTestSystem.WebApi.Controllers.Tests;

[Route("api/tests")]
[ApiController]
public class TestsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;

    public TestsController(ISender sender, IUserContext userContext)
    {
        _sender = sender;
        _userContext = userContext;
    }

    [HttpGet("{testId:guid}")]
    public async Task<IActionResult> GetTest(Guid testId, CancellationToken cancellationToken)
    {
        var query = new GetTestQuery(testId, _userContext.UserId);

        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTest(CreateTestRequest request, CancellationToken cancellationToken)
    {
        var query = new CreateTestCommand(
            request.Subject,
            request.Theme,
            request.Questions
                .Select(q => 
                    new CreateTestCommandQuestion(q.Text, q.AnswersOptions, q.TrueAnswerNumber)).ToArray(),
            _userContext.UserId);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetTest), new {id = result.Value }, result.Value);
    }
    
    [HttpDelete("{testId:guid}")]
    public async Task<IActionResult> DeleteTest(Guid testId, CancellationToken cancellationToken)
    {
        var command = new DeleteTestCommand(testId, _userContext.UserId);
        var result = await _sender.Send(command, cancellationToken);
    
        return result.IsSuccess ? NoContent() : NotFound();
    }

    [HttpGet("{q}")]
    public async Task<IActionResult> FindTest(
        [FromQuery] string q, 
        CancellationToken cancellationToken = default)
    {
        var query = new FindTestQuery(q, _userContext.UserId);
        var result = await _sender.Send(query, cancellationToken);
    
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var getOptions = TestsGetOptions.Paged(pageSize, pageNumber);
        var query = new GetTestsQuery(_userContext.UserId, getOptions);
        var result = await _sender.Send(query, cancellationToken);
    
        return Ok(result);
    }

    [HttpPut("{testId:guid}")]
    public async Task<IActionResult> UpdateTest(
        Guid testId,
        UpdateTestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTestCommand(
            testId,
            request.Subject,
            request.Theme,
            request.Questions!
                .Select(q => new UpdateTestCommandQuestion(
                    q.Text,
                    q.AnswersOptions,
                    q.TrueAnswerNumber))
                .ToArray(),
            _userContext.UserId);

        var result = await _sender.Send(command, cancellationToken);
    
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
}