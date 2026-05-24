using System.Net;
using System.Text.Json;
using Backend.Domain.Exceptions;
using Backend.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Tests.Unit.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock = new();

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JsonElement> ReadResponseJson(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return JsonDocument.Parse(body).RootElement;
    }

    [Fact]
    public async Task ApiException_Returns_CorrectStatusCodeAndBody()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw ApiException.NotFound("Produto não encontrado."),
            _loggerMock.Object);

        var context = CreateHttpContext();
        await middleware.InvokeAsync(context);

        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        var json = await ReadResponseJson(context);
        Assert.Equal("Produto não encontrado.", json.GetProperty("error").GetString());
        Assert.False(json.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrEmpty(json.GetProperty("traceId").GetString()));
    }

    [Fact]
    public async Task GenericException_Returns500_WithGenericMessage_AndNoLeaks()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new Exception("secret internal message"),
            _loggerMock.Object);

        var context = CreateHttpContext();
        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);

        var json = await ReadResponseJson(context);
        Assert.Equal("Ocorreu um erro inesperado.", json.GetProperty("error").GetString());
        Assert.False(json.GetProperty("success").GetBoolean());
        Assert.DoesNotContain("secret internal message",
            json.GetProperty("error").GetString() ?? "");
    }
}
