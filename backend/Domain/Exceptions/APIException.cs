using System.Net;

namespace Backend.Domain.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string message) : base(message)
        => StatusCode = statusCode;

    public static ApiException NotFound(string message) =>
        new(HttpStatusCode.NotFound, message);

    public static ApiException UnprocessableEntity(string message) =>
        new(HttpStatusCode.UnprocessableEntity, message);

    public static ApiException BadRequest(string message) =>
        new(HttpStatusCode.BadRequest, message);

    public static ApiException InternalServerError(string message) =>
        new(HttpStatusCode.InternalServerError, message);
}
