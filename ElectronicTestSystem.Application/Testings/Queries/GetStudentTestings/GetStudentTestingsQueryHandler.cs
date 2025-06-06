using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Users; // For IUserRepository and User
using ElectronicTestSystem.Domain.Testings; // For ITestingRepository and Testing
using ElectronicTestSystem.Domain.Tests; // For ITestRepository to get TestTitle
using ElectronicTestSystem.SDK.Data; // For PaginatedResult
using Microsoft.EntityFrameworkCore; // For IQueryable extensions like Skip, Take, ToListAsync
using System.Linq; // For LINQ methods
using System.Threading;
using System.Threading.Tasks;
using System; // For Guid
using System.Collections.Generic; // For List
using ElectronicTestSystem.Application.Abstractions.Authentication; // For IUserContext
using ElectronicTestSystem.Infrastructure; // For ApplicationDbContext (assuming it's used directly)

namespace ElectronicTestSystem.Application.Testings.Queries.GetStudentTestings;

public class GetStudentTestingsQueryHandler : IQueryHandler<GetStudentTestingsQuery, PaginatedResult<StudentTestingDto>>
{
    private readonly ApplicationDbContext _dbContext; // Using DbContext directly as per common practice in this project structure
    private readonly IUserRepository _userRepository; // To get user's group
    private readonly ITestRepository _testRepository; // To get test titles

    public GetStudentTestingsQueryHandler(ApplicationDbContext dbContext, IUserRepository userRepository, ITestRepository testRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _testRepository = testRepository;
    }

    public async Task<PaginatedResult<StudentTestingDto>> Handle(GetStudentTestingsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null || user.GroupId == null)
        {
            // Or throw a NotFoundException if that's the preferred way to handle this
            return new PaginatedResult<StudentTestingDto>(new List<StudentTestingDto>(), request.PageNumber, request.PageSize, 0);
        }

        var queryable = _dbContext.Testings
            .Where(t => t.GroupId == user.GroupId);

        var totalCount = await queryable.CountAsync(cancellationToken);

        var testings = await queryable
            .OrderByDescending(t => t.CreatedAtUtc) // Assuming there's a CreatedAtUtc or similar for ordering
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var studentTestingDtos = new List<StudentTestingDto>();

        foreach (var testing in testings)
        {
            var test = await _testRepository.GetByIdAsync(testing.TestId, cancellationToken);
            var testTitle = test?.Subject ?? "Test Title Not Found"; // Or test.Title if that's the property

            // Determine status and score (this is simplified, might need more complex logic)
            string status = testing.Status.ToString(); // Assuming TestingStatus is an enum
            int? score = null; // Placeholder for score logic

            // Check if there are user answers for this testing to determine if it's completed and calculate score
            var testingUserAnswers = await _dbContext.TestingUserAnswers
                .FirstOrDefaultAsync(ua => ua.TestingId == testing.Id && ua.UserId == request.UserId, cancellationToken);

            if (testingUserAnswers != null)
            {
                status = "Completed"; // Or use TestingStatus.Completed.ToString()
                // Placeholder for score calculation logic
                // score = CalculateScore(testingUserAnswers, test);
                // This would require access to correct answers and user's answers.
                // For now, let's assume a simple score if answers exist.
                 if (testingUserAnswers.Mark != null)
                 {
                    score = (int)testingUserAnswers.Mark.Value; // Assuming Mark has a Value property
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

        return new PaginatedResult<StudentTestingDto>(studentTestingDtos, request.PageNumber, request.PageSize, totalCount);
    }
}
