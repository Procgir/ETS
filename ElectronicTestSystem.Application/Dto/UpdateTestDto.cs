namespace ElectronicTestSystem.Application.Dto;

public sealed record UpdateTestDto(
    string? Subject,
    string? Theme,
    UpdateTestQuestionDto[]? Questions);