using BookstoreWeb.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreWeb.API.Middleware;

//catch all exceptions chưa xử lý, map sang HTTP status code đúng
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            //Log tất cả unhandled exceptions với đầy đủ context
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        //Map từng loại exception sang HTTP status code tương ứng
        var statusCode = exception switch
        {
            NotFoundException   => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            ConflictException   => StatusCodes.Status409Conflict,
            _                   => StatusCodes.Status500InternalServerError
        };

        //Trả về ProblemDetails theo chuẩn RFC 7807
        var problem = new ProblemDetails
        {
            Status   = statusCode,
            Title    = GetTitle(statusCode),
            Detail   = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode  = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }

    //Map status code sang title string
    private static string GetTitle(int statusCode) => statusCode switch
    {
        404 => "Not Found",
        400 => "Bad Request",
        409 => "Conflict",
        _   => "Internal Server Error"
    };
}
