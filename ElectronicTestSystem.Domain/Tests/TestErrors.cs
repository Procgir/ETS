using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Domain.Tests;

public static class TestErrors
{
    
    public static Error NotFound = new(
        "Test.NotFound", 
        "Test not found.");
    
    public static Error CantGetNotOwnTest = new(
        "Test.CantGetNotOwnTest", 
        "Cant get test. It's not your test.");
    
    public static Error CantDeleteNotOwnTest = new(
        "Test.CantDeleteTest", 
        "Cant delete test. It's not your test.");
    
    public static Error CantUpdateNotOwnTest = new(
        "Test.CantUpdateNotOwnTest", 
        "Cant update test. It's not your test.");
    
    public static Error CantAddQuestionWithEmptyBody = new(
    "Test.CantAddQuestionWithEmptyBody", 
    "Can't add the question to test. The body is empty.");
    
    public static Error CantAddQuestionWithEmptyAnswersOptions = new(
        "Test.CantAddQuestionWithEmptyAnswersOptions", 
        "Can't add the question to test. The answers options are empty.");

    public static Error CantAddQuestionWithIncorrectTrueAnswerNumber = new(
        "Test.CantAddQuestionWithIncorrectTrueAnswerNumber", 
        "Can't add the question to test. True answer number is incorrect.");

}