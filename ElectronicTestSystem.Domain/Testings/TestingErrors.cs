using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Domain.Testings;

public static class TestingErrors
{
    public static Error NotChanged = new(
        "Testing.NotChanged", 
        "Ended date of testing must be greater than created at date");

    public static Error NotFound = new(
        "Testing.NotFound",
        "Testing not found.");
    
    public static Error CantGetNotOwnTesting = new(
        "Testing.CantGetNotOwnTesting", 
        "Cant get testing. It is not your own.");
    
    public static Error CantGetFinishedTesting = new(
        "Testing.CantGetFinishedTesting", 
        "Cant get testing. It is finished.");
    
    public static Error CantAssignTestingUserMustBeTeacher = new(
        "Testing.CantAssignTestingUserMustBeTeacher", 
        "Cant assign testing. User must be a teacher");
    
    public static Error CantAssignTestingUserMustBeAnOwner = new(
        "Testing.CantAssignTestingTestIsNotYourOwn", 
        "Cant assign testing. User must be an owner of test");
    
    public static Error CantCreateAnswersUserIsNotInTestingGroup = new(
        "Testing.CantCreateAnswersUserIsNotInTestingGroup", 
        "Cant write answers. Testing was assigned for another group");
    
    public static Error CantCreateAnswersAnswersCountIsNotTheSameAsQuestionsCount = new(
        "Testing.CantCreateAnswersAnswersCountIsNotTheSameAsQuestionsCount", 
        "Cant write answers. Users answers count needs to be equal with count questions");
    
    public static Error CantCreateAnswersTestingWasFinished = new(
        "Testing.CantCreateAnswersTestingWasFinished", 
        "Cant write answers. Testing was finished");
    
    public static Error CantCreateAnswersItIsJustChanged = new(
        "Testing.CantCreateAnswersItIsJustChanged", 
        "Cant write answers. Testing answers was just changed");
}