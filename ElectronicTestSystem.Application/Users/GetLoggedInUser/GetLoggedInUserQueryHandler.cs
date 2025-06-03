using System.Data;
using Dapper;
using ElectronicTestSystem.Application.Abstractions.Authentication;
using ElectronicTestSystem.Application.Abstractions.Data;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Application.Users.GetLoggedInUser;

internal sealed class GetLoggedInUserQueryHandler
    : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserContext _userContext;

    public GetLoggedInUserQueryHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        IUserContext userContext)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _userContext = userContext;
    }

    public async Task<Result<UserResponse>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id AS Id,
                name_first_name AS FirstName,
                name_last_name AS LastName,
                is_teacher AS IsTeacher
            FROM users
            WHERE identity_id = @IdentityId
            """;

        UserResponse user = await connection.QuerySingleAsync<UserResponse>(
            sql,
            new
            {
                _userContext.IdentityId
            });

        return user;
    }
}
