using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ProshoreHouseBroker.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireVerifiedEmailAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var principal = context.HttpContext.User;

        if (principal.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var emailVerifiedClaim = principal.FindFirstValue("email_verified");

        if (!bool.TryParse(emailVerifiedClaim, out var isVerified) || !isVerified)
        {
            context.Result = new ForbidResult();
        }
    }
}
