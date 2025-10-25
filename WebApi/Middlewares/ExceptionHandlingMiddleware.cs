using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private static readonly Action<ILogger, string, Exception?> UnhandledExceptionLogger =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(ExceptionHandlingMiddleware)),
            "Unhandled exception in middleware: {Message}");

    private static readonly Action<ILogger, string, Exception?> DomainExceptionLogger =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(2, nameof(ExceptionHandlingMiddleware)),
            "Domain exception: {Message}");

    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ValidateContext(context);
        await this.InvokeInternalAsync(context);
    }

    private static void ValidateContext(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;

        var problem = new
        {
            type = $"https://httpstatuses.io/{(int)status}",
            title = message,
            status = (int)status,
            traceId = context.TraceIdentifier,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private async Task InvokeInternalAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            UnhandledExceptionLogger(this.logger, ex.Message, ex);
            await WriteErrorAsync(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            UnhandledExceptionLogger(this.logger, ex.Message, ex);
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            DomainExceptionLogger(this.logger, ex.Message, ex);
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            DomainExceptionLogger(this.logger, ex.Message, ex);
            await WriteErrorAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
    }
}
