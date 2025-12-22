namespace UrlShortener.Shared.Common.Result;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Message { get; }
    
    public string? Error => IsSuccess ? null : Message;

    public int Status { get; }

    private Result(T value, string? message = null, int status = 200)
    {
        IsSuccess = true;
        Value = value;
        Message = message;
        Status = status;
    }

    private Result(string error, int status = 400)
    {
        IsSuccess = false;
        Message = error;
        Status = status;
    }

    // SUCCESS
    public static Result<T> Success(T value) =>
        new Result<T>(value);

    public static Result<T> Success(T value, string message) =>
        new Result<T>(value, message);

    public static Result<T> Success(T value, string message, int status) =>
        new Result<T>(value, message, status);

    public static Result<T> Success(T value, int status) =>
        new Result<T>(value, null, status);

    // FAILURE
    public static Result<T> Failure(string error) =>
        new Result<T>(error);

    public static Result<T> Failure(string error, int status) =>
        new Result<T>(error, status);
}
