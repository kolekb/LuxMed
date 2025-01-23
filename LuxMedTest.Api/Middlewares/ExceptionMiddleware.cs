using FluentValidation;
using LuxMedTest.Application.Enums;
using System.Net;

namespace LuxMedTest.Api.Middlewares;
public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ErrorType.ValidationError, ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, ErrorType.Unauthorized, ex.Message);
        }
        catch (Exception)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ErrorType.InternalError, "An unexpected error occurred.");
        }
    }

    private Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, ErrorType errorType, object details)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            errorType = errorType.ToString(),
            details
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
