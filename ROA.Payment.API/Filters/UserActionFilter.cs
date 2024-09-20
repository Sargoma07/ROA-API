using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using ROA.Payment.API.Data;

namespace ROA.Payment.API.Filters;

public class UserActionFilter(IAccountContext accountContext) : IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            accountContext.AccountId = userIdClaim.Value;
        }

        await next();
    }
}
