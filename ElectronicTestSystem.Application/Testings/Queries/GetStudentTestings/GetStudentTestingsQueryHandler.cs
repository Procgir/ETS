using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto; // For StudentTestingDto and PagedResultResponse
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ElectronicTestSystem.Infrastructure; // For ApplicationDbContext
// Remove: using ElectronicTestSystem.SDK.Data;
// Remove: using ElectronicTestSystem.Application.Abstractions.Authentication; // IUserContext was not used

namespace ElectronicTestSystem.Application.Testings.Queries.GetStudentTestings;

// Changed return type to PagedResultResponse<StudentTestingDto>
public class GetStudentTestingsQueryHandler : IQueryHandler<GetStudentTestingsQuery, PagedResultResponse<StudentTestingDto>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    // private readonly ITestRepository _testRepository; // Test repository is not strictly needed if Test details are part of Testing or can be joined efficiently

    public GetStudentTestingsQueryHandler(ApplicationDbContext dbContext, IUserRepository userRepository) // Removed ITestRepository for now
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    // Changed return type to PagedResultResponse<StudentTestingDto>
    public async Task<PagedResultResponse<StudentTestingDto>> Handle(GetStudentTestingsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        // If user or group is not found, return an empty paginated response
        if (user == null || user.GroupId == null)
        {
            var emptyPagerInfo = new PagedResultResponsePagerInfo(request.PageNumber, request.PageSize, 0, 0);
            return new PagedResultResponse<StudentTestingDto>(Array.Empty<StudentTestingDto>(), emptyPagerInfo);
        }

        // Base query for testings assigned to the user's group
        var testingsQuery = _dbContext.Testings
            .Where(t => t.GroupId == user.GroupId);

        var totalCount = await testingsQuery.CountAsync(cancellationToken);

        // Apply pagination and ordering
        // It's often better to join and select into DTO before pagination if possible,
        // but here we fetch entities then map, which is fine for moderate amounts of data per page.
        var testingsOnPage = await testingsQuery
            .OrderByDescending(t => t.CreatedAtUtc) // Assuming CreatedAtUtc exists for ordering
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var studentTestingDtos = new List<StudentTestingDto>();

        foreach (var testing in testingsOnPage)
        {
            // Attempt to get the Test's title.
            // This assumes 'Test' is a navigation property on 'Testing' entity or can be efficiently loaded.
            // If 'Test' is not directly navigable or causes N+1, a join approach or separate batched query for test details would be better.
            // For simplification as requested, direct navigation or existing eager load is assumed for now.
            var testEntity = await _dbContext.Tests.FindAsync(new object[] { testing.TestId }, cancellationToken);
            var testTitle = testEntity?.Subject ?? "N/A"; // Using Subject as title based on previous DTOs

            string status = testing.Status.ToString(); // Assumes TestingStatus is an enum
            int? score = null;

            // Check for user's answers to this specific testing
            var testingUserAnswer = await _dbContext.TestingUserAnswers
                .FirstOrDefaultAsync(ua => ua.TestingId == testing.Id && ua.UserId == request.UserId, cancellationToken);

            if (testingUserAnswer != null)
            {
                // If answers exist, the testing is considered "Completed" for this user.
                // This could also come from testing.Status if it reflects individual completion.
                status = Domain.Testings.TestingStatus.Completed.ToString(); // Using the enum value directly for "Completed"
                if (testingUserAnswer.Mark != null)
                {
                    score = (int)testingUserAnswer.Mark.Value; // Assumes Mark has a Value property
                }
            }

            studentTestingDtos.Add(new StudentTestingDto
            {
                TestingId = testing.Id,
                TestTitle = testTitle,
                Status = status,
                Score = score
            });
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var pagerInfo = new PagedResultResponsePagerInfo(request.PageNumber, request.PageSize, totalCount, totalPages);

        return new PagedResultResponse<StudentTestingDto>(studentTestingDtos.ToArray(), pagerInfo);
    }
}
