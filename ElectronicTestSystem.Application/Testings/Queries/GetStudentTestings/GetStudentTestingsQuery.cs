using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.SDK.Data; // Assuming PaginatedResult is here

namespace ElectronicTestSystem.Application.Testings.Queries.GetStudentTestings;

public record GetStudentTestingsQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IQuery<PaginatedResult<StudentTestingDto>>;
