using ElectronicTestSystem.Application.Abstractions.Authentication;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Users.LogInUser;

internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
    private readonly IJwtService _jwtService;

    public LogInUserCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<Result<AccessTokenResponse>> Handle(
        LogInUserCommand request,
        CancellationToken cancellationToken)
    {
        Result<string> result = await _jwtService.GetAccessTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);
        }

        return new AccessTokenResponse(result.Value);
    }
}
