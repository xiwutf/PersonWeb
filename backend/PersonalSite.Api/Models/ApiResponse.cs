namespace PersonalSite.Api.Models;

/// <summary>
/// 统一 API 响应格式
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiResponse<T>
{
    public int Code { get; set; } = 0;
    public string Message { get; set; } = "ok";
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "ok")
    {
        return new ApiResponse<T> { Code = 0, Message = message, Data = data };
    }

    public static ApiResponse<T> Error(string message, int code = -1)
    {
        return new ApiResponse<T> { Code = code, Message = message, Data = default };
    }
}

public class ApiResponse
{
    public int Code { get; set; } = 0;
    public string Message { get; set; } = "ok";
    public object? Data { get; set; }

    public static ApiResponse Success(object? data = null, string message = "ok")
    {
        return new ApiResponse { Code = 0, Message = message, Data = data };
    }

    public static ApiResponse Error(string message, int code = -1)
    {
        return new ApiResponse { Code = code, Message = message, Data = null };
    }
}
