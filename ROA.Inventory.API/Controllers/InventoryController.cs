using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROA.Infrastructure.Data;
using ROA.Inventory.API.Data;
using ROA.Inventory.API.Data.Repositories;
using ROA.Inventory.API.Domain.Types;
using ROA.Inventory.API.Mappers;
using ROA.Inventory.API.Models;

namespace ROA.Inventory.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class InventoryController(
        IPlayerContext playerContext,
        IDataContextManager dataContextManager,
        IMapperFactory mapperFactory,
        ILogger<InventoryController> logger
    )
        : AbstractController(dataContextManager, mapperFactory, logger)
    {
        [HttpGet("")]
        public async Task<ActionResult<InventoryModel>> GetInventory()
        {
            var playerId = playerContext.PlayerId;

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetInventory(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            var mapper = MapperFactory.GetMapper<IInventoryMapper>();
            return mapper.MapToDto(inventory);
        }

        [HttpGet("storage")]
        public async Task<ActionResult<InventoryModel>> GetStorage()
        {
            var playerId = playerContext.PlayerId;

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetStorage(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            var mapper = MapperFactory.GetMapper<IInventoryMapper>();
            return mapper.MapToDto(inventory);
        }

        [HttpGet("equipment")]
        public async Task<ActionResult<InventoryModel>> GetEquipment()
        {
            var playerId = playerContext.PlayerId;

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetEquipment(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            var mapper = MapperFactory.GetMapper<IInventoryMapper>();
            return mapper.MapToDto(inventory);
        }

        [HttpPut("")]
        public async Task<ActionResult<InventoryModel>> UpdateInventory([FromBody] InventoryModel request)
        {
            var playerId = playerContext.PlayerId;

            using var _ = Logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var mapper = MapperFactory.GetMapper<IInventoryMapper>();

            var inventory = await inventoryRepository.GetInventory(playerId);

            if (inventory == null)
            {
                inventory = mapper.MapFromDto(request);
                inventory.PlayerId = playerId;
                inventory.Type = InventoryType.CharacterInventory;
            }
            else
            {
                inventory = mapper.MapFromDto(request, destination: inventory);
            }

            inventoryRepository.AddOrUpdate(inventory);

            await DataContextManager.SaveAsync();

            Logger.LogInformation("Inventory {inventoryId} updated", inventory.Id);
            return mapper.MapToDto(inventory);
        }

        [HttpPut("storage")]
        public async Task<ActionResult<InventoryModel>> UpdateStorage([FromBody] InventoryModel request)
        {
            var playerId = playerContext.PlayerId;

            using var _ = Logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var mapper = MapperFactory.GetMapper<IInventoryMapper>();

            var inventory = await inventoryRepository.GetStorage(playerId);

            if (inventory == null)
            {
                inventory = mapper.MapFromDto(request);
                inventory.PlayerId = playerId;
                inventory.Type = InventoryType.Storage;
            }
            else
            {
                inventory = mapper.MapFromDto(request, destination: inventory);
            }

            inventoryRepository.AddOrUpdate(inventory);

            await DataContextManager.SaveAsync();

            Logger.LogInformation("Storage {inventoryId} updated", inventory.Id);
            return mapper.MapToDto(inventory);
        }

        [HttpPut("equipment")]
        public async Task<ActionResult<InventoryModel>> UpdateEquipment([FromBody] InventoryModel request)
        {
            var playerId = playerContext.PlayerId;

            using var _ = Logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var mapper = MapperFactory.GetMapper<IInventoryMapper>();

            var inventory = await inventoryRepository.GetEquipment(playerId);

            if (inventory == null)
            {
                inventory = mapper.MapFromDto(request);
                inventory.PlayerId = playerId;
                inventory.Type = InventoryType.Equipment;
            }
            else
            {
                inventory = mapper.MapFromDto(request, destination: inventory);
            }

            inventoryRepository.AddOrUpdate(inventory);

            await DataContextManager.SaveAsync();

            Logger.LogInformation("Equipment {inventoryId} updated", inventory.Id);
            return mapper.MapToDto(inventory);
        }
    }
}