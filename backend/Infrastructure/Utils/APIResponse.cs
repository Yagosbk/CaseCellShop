namespace Backend.Infrastructure.Utils;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? TraceId { get; set; }

    public static ApiResponse Fail(string error, string traceId)
    {
        return new ApiResponse { Success = false, Error = error, TraceId = traceId };
    }
}
