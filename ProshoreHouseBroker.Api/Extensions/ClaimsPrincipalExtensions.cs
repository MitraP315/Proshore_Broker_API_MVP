using System.Security.Claims;

namespace ProshoreHouseBroker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var raw = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    public static Guid GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var id = principal.GetUserId();

        if (!id.HasValue)
        {
            throw new UnauthorizedAccessException("User id was not found in the token.");
        }

        return id.Value;
    }
}
