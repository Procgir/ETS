using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto; // Ensure PagedResultResponse is accessible

namespace ElectronicTestSystem.Application.Testings.Queries.GetStudentTestings;

public record GetStudentTestingsQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IQuery<PagedResultResponse<StudentTestingDto>>;
