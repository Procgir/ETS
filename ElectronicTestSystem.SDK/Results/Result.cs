namespace ElectronicTestSystem.SDK.Results;

public struct Result<T>
{
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess { get; }

    private Result(T? value, string? errorMessage, bool isSuccess) => (Value, ErrorMessage, IsSuccess) = (value, errorMessage, isSuccess);

    public static Result<T> Success(T value) => new(value, null, true);
    public static Result<T> Fail(string errorMessage) => new(default, errorMessage, false);
}
