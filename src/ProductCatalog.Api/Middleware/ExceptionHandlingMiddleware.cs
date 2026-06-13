using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ProductCatalog.Api.Common;

namespace ProductCatalog.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteValidationProblemAsync(context, exception);
        }
        catch (NotFoundException exception)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status404NotFound,
                "Объект не найден",
                exception.Message);
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "Конфликт данных",
                "Объект с такими уникальными данными уже существует.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Внутренняя ошибка сервера",
                "При обработке запроса произошла непредвиденная ошибка.");
        }
    }

    private static async Task WriteValidationProblemAsync(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ошибка валидации",
            Detail = "Проверьте переданные данные."
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int status,
        string title,
        string detail)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
