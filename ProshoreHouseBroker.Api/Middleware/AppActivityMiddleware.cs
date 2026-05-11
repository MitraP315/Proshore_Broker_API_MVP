using ProshoreHouseBroker.Api.Extensions;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Infrastructure.Persistence;

namespace ProshoreHouseBroker.Api.Middleware;

public class AppActivityMiddleware
{
    private readonly RequestDelegate _next;

    public AppActivityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            if (!ShouldSkip(context))
            {
                try
                {
                    dbContext.AppActivities.Add(new AppActivity
                    {
                        Id = Guid.NewGuid(),
                        Path = context.Request.Path.Value ?? string.Empty,
                        Method = context.Request.Method,
                        StatusCode = context.Response.StatusCode,
                        EndpointName = context.GetEndpoint()?.DisplayName,
                        UserId = context.GetAuditUserId(),
                        DeviceId = context.GetDeviceId(),
                        IpAddress = context.GetIpAddress(),
                        CreatedAtUtc = DateTime.UtcNow
                    });

                    await dbContext.SaveChangesAsync();
                }
                catch
                {
                    // Activity logging must never break the request pipeline.
                }
            }
        }
    }

    private static bool ShouldSkip(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        return path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase);
    }
}
