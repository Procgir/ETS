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
using ElectronicTestSystem.Application.Dto; // For StudentTestingDto
using ElectronicTestSystem.SDK.Data; // For PaginatedResult

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

    [HttpGet("student")] // Changed path to "student" to avoid conflict with existing GetTestings and to be more descriptive
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(PaginatedResult<StudentTestingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentTestings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("User ID not found in token or is invalid.");
        }

        var query = new GetStudentTestingsQuery(userId, pageNumber, pageSize);

        var result = await _sender.Send(query, cancellationToken);

        // Assuming the handler returns PaginatedResult<StudentTestingDto> directly,
        // and not a Result<PaginatedResult<StudentTestingDto>> like other queries seem to do.
        // If it returns Result<T>, the success check should be:
        // if (result.IsFailure) return NotFound(); // Or BadRequest based on error type
        // return Ok(result.Value);

        // For now, assuming direct return or that the handler itself manages the Result wrapper.
        // If PaginatedResult can be null or empty in a "not found" an explicit check might be better.
        if (result == null || !result.Items.Any()) // Basic check if no items are returned
        {
            // Consider if an empty list is a valid 200 OK or a 404 Not Found.
            // The handler returns an empty list if user/group not found, so 200 OK with empty list is current behavior.
        }

        return Ok(result);
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