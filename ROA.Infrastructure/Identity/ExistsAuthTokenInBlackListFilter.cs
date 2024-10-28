using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ROA.Infrastructure.Identity;

public class ExistsAuthTokenInBlackListFilter(
    IDistributedCache cache,
    ILogger<ExistsAuthTokenInBlackListFilter> logger
) : IAsyncAuthorizationFilter
{
    private const string BlackListTokenCacheName = "AccessToken:BlackList";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var token = GetAccessToken(context);

        if (token is null)
        {
            context.Result = new UnauthorizedResult();
        }
        
        var key = $"{BlackListTokenCacheName}:{token}";
        var result = await cache.GetStringAsync(key);
        
        if (result is not null)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private string? GetAccessToken(AuthorizationFilterContext context)
    {
        var identity = context.HttpContext.User.Identity as CaseSensitiveClaimsIdentity;

        if (identity?.SecurityToken is not JsonWebToken jwt)
        {
            logger.LogWarning("Incorrect token");
            return null; 
        }

        return jwt + "." + jwt.EncodedSignature;
    }
}