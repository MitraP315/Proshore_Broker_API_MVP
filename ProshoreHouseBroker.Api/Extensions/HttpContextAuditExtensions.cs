using System.Security.Claims;

namespace ProshoreHouseBroker.Api.Extensions;

public static class HttpContextAuditExtensions
{
    public static string? GetAuditUserId(this HttpContext context)
        => context.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetDeviceId(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Device-Id", out var value))
        {
            return value.FirstOrDefault();
        }

        return null;
    }

    public static string? GetIpAddress(this HttpContext context)
        => context.Connection.RemoteIpAddress?.ToString();
}
