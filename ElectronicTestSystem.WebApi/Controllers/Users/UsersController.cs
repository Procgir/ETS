using Asp.Versioning;
using ElectronicTestSystem.Application.Users.GetLoggedInUser;
using ElectronicTestSystem.Application.Users.LogInUser;
using ElectronicTestSystem.Application.Users.RegisterUser;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicTestSystem.WebApi.Controllers.Users;

[ApiController]
[ApiVersion(ApiVersions.V1, Deprecated = true)]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    [MapToApiVersion(ApiVersions.V1)]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUserV1(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        Result<UserResponse> result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }
    
    [HttpGet("me")]
    [MapToApiVersion(ApiVersions.V2)]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUserV2(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        Result<UserResponse> result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Login,
            request.FirstName,
            request.LastName,
            request.Password,
            request.IsTeacher,
            request.GroupId);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(
        LogInUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Login, request.Password);

        Result<AccessTokenResponse> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }
}
