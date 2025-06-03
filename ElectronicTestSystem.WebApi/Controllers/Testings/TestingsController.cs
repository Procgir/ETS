using ElectronicTestSystem.Application.Testings.AssignTesting;
using ElectronicTestSystem.Application.Testings.CreateTestingAnswers;
using ElectronicTestSystem.Application.Testings.DeleteActiveTesting;
using ElectronicTestSystem.Application.Testings.GetTesting;
using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Application.Testings.SearchTestings;
using ElectronicTestSystem.Domain.Testings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicTestSystem.WebApi.Controllers.Testings;

[ApiController]
[Route("api/testings")]
public class TestingsController : ControllerBase
{
    private readonly ISender _sender;

    public TestingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{testingId:guid}")]
    public async Task<IActionResult> GetTesting(Guid testingId, CancellationToken cancellationToken)
    {
        var query = new GetTestingQuery(testingId, default);

        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetTestings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var getOptions = TestingsGetOptions.Paged(pageSize, pageNumber);
        var query = new GetTestingsQuery(default, getOptions);

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignTesting([FromBody] AssignTestingRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignTestingCommand(
            request.TestId,
            request.GroupId,
            default,
            request.EndDate);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{testingId:guid}/answers")]
    public async Task<IActionResult> CreateTestingAnswers(Guid testingId, [FromBody] CreateTestingAnswersRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTestingAnswersCommand(
            default,
            testingId,
            request.Answers);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result);
    }
    
    [HttpDelete("{testingId:guid}")]
    public async Task<IActionResult> DeleteActiveTesting(Guid testingId, CancellationToken cancellationToken)
    {
        var command = new DeleteActiveTestingCommand(testingId);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTestings([FromQuery] string q, CancellationToken cancellationToken)
    {
        var query = new SearchTestingsQuery(q);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }
}