namespace UrlShortener.Shared.Common.Result;

public class Result
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public int Status { get; }

    private Result(bool isSuccess, string? message = null, int status = 200)
    {
        IsSuccess = isSuccess;
        Message = message;
        Status = status;
    }

    // SUCCESS
    public static Result Success() =>
        new Result(true, null, 200);

    public static Result Success(string message) =>
        new Result(true, message, 200);

    public static Result Success(string message, int status) =>
        new Result(true, message, status);

    public static Result Success(int status) =>
        new Result(true, null, status);

    // FAILURE
    public static Result Failure(string message) =>
        new Result(false, message, 400);

    public static Result Failure(string message, int status) =>
        new Result(false, message, status);
}
