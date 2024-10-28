using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using ROA.Inventory.API.Data;

namespace ROA.Inventory.API.Filters;

public class UserActionFilter(IPlayerContext playerContext) : IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            playerContext.PlayerId = userIdClaim.Value;
        }

        await next();
    }
}
