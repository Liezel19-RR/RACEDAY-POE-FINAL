using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SessionAuthorizeAttribute(params string[] roles) : Attribute, IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        if (string.IsNullOrWhiteSpace(session.GetString(SessionKeys.Role)))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return Task.CompletedTask;
        }
        if (roles.Length > 0 && !roles.Contains(session.GetString(SessionKeys.Role), StringComparer.OrdinalIgnoreCase))
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        return Task.CompletedTask;
    }
}
