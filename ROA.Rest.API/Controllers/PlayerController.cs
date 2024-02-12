using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class PlayerController(IDataContextManager dataContextManager, ILogger<PlayerController> logger)
    : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<Player>> SignUp([FromBody] PlayerSignUp request)
    {
        using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", request.ExternalId } });

        var playerRepository = dataContextManager.CreateRepository<IPlayerRepository>();
        var player = await playerRepository.GetByExternalId(request.ExternalId);

        if (player is null)
        {
            player = Player.Create(request.ExternalId, request.Provider);
            playerRepository.AddOrUpdate(player);
            await dataContextManager.SaveAsync();
        }

        logger.LogInformation("Storage {playerId} updated", player.Id);
        return player;
    }
}