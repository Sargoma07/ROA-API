using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ROA.Domain.Events;
using ROA.Identity.API.Data.Repositories;
using ROA.Identity.API.Domain;
using ROA.Identity.API.Domain.Dtos;
using ROA.Identity.API.EventBus;
using ROA.Identity.API.Mappers;
using ROA.Identity.API.Models;
using ROA.Identity.API.Settings;
using ROA.Infrastructure.Data;

namespace ROA.Identity.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IUserCreatedProducer userCreatedProducer,
    IOptions<AuthSettings> settings,
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger<AuthController> logger)
    : AbstractController(dataContextManager, mapperFactory, logger)
{
    [HttpPost("signup")]
    public async Task<ActionResult<TokenModel>> SignUp([FromBody] SignUpModel request)
    {
        using var _ = Logger.BeginScope(new Dictionary<string, object> { { "UserId", request.ExternalId } });

        var userRepository = DataContextManager.CreateRepository<IUserRepository>();
        var user = await userRepository.GetByExternalId(request.ExternalId);

        if (user is not null)
        {
            Logger.LogWarning("User already exists");
            return Problem("Invalid auth data", statusCode: 400);
        }

        var mapper = MapperFactory.GetMapper<IUserMapper>();
        user = mapper.MapFromDto(request);
        userRepository.AddOrUpdate(user);
        await DataContextManager.SaveAsync();

        await userCreatedProducer.ProduceAsync(new UserCreatedEvent()
        {
            UserId = user.Id
        });

        var authData = new AuthDataDto()
        {
            ExternalId = request.ExternalId,
        };

        try
        {
            return await CreateToken(authData);
        }
        catch (InvalidOperationException)
        {
            return Problem("Invalid auth data", statusCode: 401);
        }
    }

    [HttpPost("signin")]
    public async Task<ActionResult<TokenModel>> SignIn([FromBody] SignInModel request)
    {
        var authData = new AuthDataDto()
        {
            ExternalId = request.ExternalId,
        };

        try
        {
            return await CreateToken(authData);
        }
        catch (InvalidOperationException)
        {
            return Problem("Invalid auth data", statusCode: 401);
        }
    }

    [HttpPost]
    [Route("refreshToken")]
    public async Task<ActionResult<TokenModel>> RefreshToken([FromBody] RefreshTokenModel request)
    {
        var claimContainer = await GetClaimsFromExpiredToken(request.Access);

        var nameClaim = claimContainer.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

        if (nameClaim is null)
        {
            throw new InvalidOperationException("Name claim not found");
        }

        using var _ = Logger.BeginScope(new Dictionary<string, string>() { { "UserExternalId", nameClaim.Value } });

        var userRepository = DataContextManager.CreateRepository<IUserRepository>();
        var user = await userRepository.GetByExternalId(nameClaim.Value);

        if (user is null)
        {
            Logger.LogInformation("User not found by external id");
            return Unauthorized();
        }

        if (!user.RefreshTokenSessions.ContainsKey(request.Refresh))
        {
            Logger.LogInformation("Refresh token not found");
            return Unauthorized();
        }

        user.RefreshTokenSessions.Remove(request.Refresh);

        var refreshToken = GenerateRefreshToken();
        var refreshExpires = DateTime.UtcNow.AddDays(settings.Value.RefreshExpiresDays);

        var authClaims = claimContainer.Claims.Where(x => x.Type != "aud");
        var token = CreateToken(authClaims);

        user.RefreshTokenSessions.Add(refreshToken, new User.RefreshTokenSession
        {
            RefreshExpires = refreshExpires,
            AccessToken = token
        });

        userRepository.AddOrUpdate(user);
        await DataContextManager.SaveAsync();

        return new TokenModel()
        {
            Access = token,
            Refresh = refreshToken,
        };
    }

    private async Task<TokenModel> CreateToken(AuthDataDto authData)
    {
        using var _ = Logger.BeginScope(new Dictionary<string, string>() { { "UserExternalId", authData.ExternalId } });

        var userRepository = DataContextManager.CreateRepository<IUserRepository>();
        var user = await userRepository.GetByExternalId(authData.ExternalId);

        if (user is null)
        {
            throw new InvalidOperationException("User not found by external id");
        }

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, authData.ExternalId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id),
        };

        var token = CreateToken(authClaims);

        var refreshToken = GenerateRefreshToken();
        var refreshExpires = DateTime.UtcNow.AddDays(settings.Value.RefreshExpiresDays);

        user.RefreshTokenSessions.Add(refreshToken, new User.RefreshTokenSession
        {
            RefreshExpires = refreshExpires,
            AccessToken = token
        });

        userRepository.AddOrUpdate(user);
        await DataContextManager.SaveAsync();

        Logger.LogInformation("Token created");

        return new TokenModel()
        {
            Access = token,
            Refresh = refreshToken,
        };
    }

    private string CreateToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Value.Secret));

        var claims = authClaims.ToDictionary(x => x.Type, object (x) => x.Value);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = settings.Value.Issuer,
            Audience = settings.Value.Audience,
            Claims = claims,
            IssuedAt = null,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.Now.AddMinutes(settings.Value.TokenExpireMinutes),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        };

        var handler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };

        var token = handler.CreateToken(descriptor);

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<ClaimsIdentity> GetClaimsFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = settings.Value.Audience,
            ValidateAudience = true,
            ValidIssuer = settings.Value.Issuer,
            ValidateIssuer = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Value.Secret)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false
        };

        var tokenHandler = new JsonWebTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);

        return result.ClaimsIdentity;
    }
}