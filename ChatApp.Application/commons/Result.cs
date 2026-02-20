using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Application.commons;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Result<T>
{
    public string Message { get; set; } = string.Empty;

    public T? Value { get; set; }

    public bool IsSuccessful { get; set; }

    public List<string> ValidationErrors { get; set; } = [];

    public string ResponseTime { get; set; } = string.Empty;
    
    public Result() { }

    private Result(bool isSuccessful, T value, string message)
    {
        IsSuccessful = isSuccessful;
        Value = value;
        Message = message;
        ResponseTime = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
    }

    private Result(bool isSuccessful, string message)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        ResponseTime = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
    }

    private Result(bool isSuccessful, string message, List<string> validationErrors)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        ValidationErrors = validationErrors;
        ResponseTime = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static Result<T> Success(string message) => new(true, message);

    public static Result<T> Success(T value, string message) => new(true, value, message);

    public static Result<T> Failure(string message) => new(false, message);

    public static Result<T> ValidationFailure(string message, List<string> validationErrors) =>
        new(false, message, validationErrors);
}