using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Rest.API.Data;
using ROA.Rest.API.Data.Repositories;
using ROA.Rest.API.Domain;
using ROA.Rest.API.Mappers;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class PlayerController(
    IDataContextManager dataContextManager,
    IMapperFactory mapperFactory,
    ILogger<PlayerController> logger)
    : AbstractController(dataContextManager, mapperFactory, logger)
{
    [HttpPost("signup")]
    public async Task<ActionResult<Player>> SignUp([FromBody] PlayerSignUpModel request)
    {
        using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", request.ExternalId } });

        var playerRepository = DataContextManager.CreateRepository<IPlayerRepository>();
        var player = await playerRepository.GetByExternalId(request.ExternalId);

        if (player is null)
        {
            player = Player.Create(request.ExternalId, request.Provider);
            playerRepository.AddOrUpdate(player);
            await DataContextManager.SaveAsync();
        }

        logger.LogInformation("Storage {playerId} updated", player.Id);
        return player;
    }

    // TODO: replace to get playerId on JWT 
    [HttpGet("{playerId}")]
    public async Task<ActionResult<PlayerModel>> GetPlayerInfo(string playerId)
    {
        var playerRepository = DataContextManager.CreateRepository<IPlayerRepository>();
        var player = await playerRepository.GetByIdAsync(playerId);

        if (player is null)
        {
            return NotFound();
        }

        var mapper = MapperFactory.GetMapper<IPlayerMapper>();
        return mapper.MapToDto(player);
    }
}