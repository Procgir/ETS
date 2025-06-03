namespace ElectronicTestSystem.Application.Dto;

public sealed record UpdateTestQuestionDto(
    string Body,
    int TrueAnswerNumber,
    string[] Answers);