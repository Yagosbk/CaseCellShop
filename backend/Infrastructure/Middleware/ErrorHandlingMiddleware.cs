using System.Net;
using System.Text.Json;
using Backend.Domain.Exceptions;
using Backend.Infrastructure.Utils;

namespace Backend.Infrastructure.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogWarning(ex,
                "[{TraceId}] Expected error on {Method} {Path} — {StatusCode}: {Message}",
                traceId, context.Request.Method, context.Request.Path,
                (int)ex.StatusCode, ex.Message);

            await WriteResponse(context, ex.StatusCode,
                ApiResponse.Fail(ex.Message, traceId));
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex,
                "[{TraceId}] Unhandled exception on {Method} {Path}",
                traceId, context.Request.Method, context.Request.Path);

            await WriteResponse(context, HttpStatusCode.InternalServerError,
                ApiResponse.Fail("Ocorreu um erro inesperado.", traceId));
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, object body)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
