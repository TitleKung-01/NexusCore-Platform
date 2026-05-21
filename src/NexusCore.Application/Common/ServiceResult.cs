namespace NexusCore.Application.Common;

public record ServiceResult<T>(bool Success, T? Data, string? Error, int StatusCode = 400)
{
    public static ServiceResult<T> Ok(T data) => new(true, data, null, 200);
    public static ServiceResult<T> Fail(string error, int statusCode = 400) => new(false, default, error, statusCode);
}
