using FluentValidation;
using ProshoreHouseBroker.Api.Extensions;
using ProshoreHouseBroker.Application.Exceptions;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Infrastructure.Persistence;
using System.Net;
using System.Text.Json;

namespace ProshoreHouseBroker.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await LogErrorAsync(context, dbContext, ex);
            await WriteAsync(context, HttpStatusCode.BadRequest, ex.Errors.Select(x => x.ErrorMessage));
        }
        catch (NotFoundException ex)
        {
            await LogErrorAsync(context, dbContext, ex);
            await WriteAsync(context, HttpStatusCode.NotFound, [ex.Message]);
        }
        catch (ForbiddenException ex)
        {
            await LogErrorAsync(context, dbContext, ex);
            await WriteAsync(context, HttpStatusCode.Forbidden, [ex.Message]);
        }
        catch (UnauthorizedAccessException ex)
        {
            await LogErrorAsync(context, dbContext, ex);
            await WriteAsync(context, HttpStatusCode.Unauthorized, [ex.Message]);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(context, dbContext, ex);
            await WriteAsync(context, HttpStatusCode.InternalServerError, ["An unexpected error occurred."]);
        }
    }

    private static async Task WriteAsync(HttpContext context, HttpStatusCode statusCode, IEnumerable<string> errors)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors }));
    }

    private static async Task LogErrorAsync(HttpContext context, ApplicationDbContext dbContext, Exception ex)
    {
        try
        {
            dbContext.ErrorInfos.Add(new ErrorInfo
            {
                Id = Guid.NewGuid(),
                Message = ex.Message,
                ExceptionType = ex.GetType().FullName ?? ex.GetType().Name,
                StackTrace = ex.StackTrace,
                Path = context.Request.Path.Value ?? string.Empty,
                Method = context.Request.Method,
                UserId = context.GetAuditUserId(),
                DeviceId = context.GetDeviceId(),
                IpAddress = context.GetIpAddress(),
                CreatedAtUtc = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }
        catch
        {
            // Error logging must never hide the original request failure.
        }
    }
}
