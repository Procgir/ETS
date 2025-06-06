using ElectronicTestSystem.Application.Testings.AssignTesting;
using ElectronicTestSystem.Application.Testings.CreateTestingAnswers;
using ElectronicTestSystem.Application.Testings.DeleteActiveTesting;
using ElectronicTestSystem.Application.Testings.GetTesting;
using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Application.Testings.SearchTestings;
using ElectronicTestSystem.Domain.Testings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
// Add these using statements at the top if they are not already present
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ElectronicTestSystem.Application.Testings.Queries.GetStudentTestings; // Adjust if the namespace is different
using ElectronicTestSystem.Application.Dto; // For StudentTestingDto and PagedResultResponse
// Remove using ElectronicTestSystem.SDK.Data; if it was there for PaginatedResult

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

    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    // Updated ProducesResponseType to use PagedResultResponse
    [ProducesResponseType(typeof(PagedResultResponse<StudentTestingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // Consider if 404 is still applicable or if empty list with 200 is always returned by handler
    public async Task<IActionResult> GetStudentTestings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            // Return Unauthorized if the user ID is not present or invalid,
            // as this indicates an issue with the authentication token or its processing.
            return Unauthorized("User ID not found in token or is invalid.");
        }

        var query = new GetStudentTestingsQuery(userId, pageNumber, pageSize);

        // The handler GetStudentTestingsQueryHandler now returns PagedResultResponse<StudentTestingDto> directly.
        // It does not wrap it in a Domain.Result object like some other handlers might.
        // So, we directly await the response.
        var pagedResult = await _sender.Send(query, cancellationToken);

        // The handler is designed to return an empty list within PagedResultResponse
        // if no data is found (e.g., user not found, group not found, or no testings for the group).
        // Thus, a null check on pagedResult itself might not be necessary if the handler guarantees non-null response.
        // An empty result (e.g., pagedResult.Result.Length == 0) is a valid 200 OK scenario.

        return Ok(pagedResult);
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