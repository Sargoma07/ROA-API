using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Rest.API.Data;
using ROA.Rest.API.Data.Repositories;
using ROA.Rest.API.Domain;
using ROA.Rest.API.Mappers;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(
        IDataContextManager dataContextManager,
        IMapperFactory mapperFactory,
        ILogger<InventoryController> logger)
        : AbstractController(dataContextManager, mapperFactory, logger)
    {
        [HttpGet("player/{playerId}/inventory")]
        public async Task<ActionResult<Inventory>> GetInventory(string playerId)
        {
            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetInventory(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        [HttpGet("player/{playerId}/storage")]
        public async Task<ActionResult<Inventory>> GetStorage(string playerId)
        {
            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetStorage(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        [HttpGet("player/{playerId}/equipment")]
        public async Task<ActionResult<Inventory>> GetEquipment(string playerId)
        {
            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetEquipment(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        [HttpPut("player/{playerId}/inventory")]
        public async Task<ActionResult<Inventory>> UpdateInventory(string playerId,
            [FromBody] InventoryUpdateSlotsModel request)
        {
            using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetInventory(playerId);

            if (inventory == null)
            {
                var playerRepository = DataContextManager.CreateRepository<IPlayerRepository>();
                var player = await playerRepository.GetByIdAsync(playerId);

                if (player is null)
                {
                    return NotFound();
                }

                inventory = Inventory.CreateInventory(playerId, request.Slots);
            }
            else
            {
                inventory.Slots = request.Slots;
            }

            inventoryRepository.AddOrUpdate(inventory);

            await DataContextManager.SaveAsync();

            logger.LogInformation("Inventory {inventoryId} updated", inventory.Id);
            return inventory;
        }

        [HttpPut("player/{playerId}/storage")]
        public async Task<ActionResult<Inventory>> UpdateStorage(string playerId,
            [FromBody] InventoryUpdateSlotsModel request)
        {
            using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetStorage(playerId);

            if (inventory == null)
            {
                var playerRepository = DataContextManager.CreateRepository<IPlayerRepository>();
                var player = await playerRepository.GetByIdAsync(playerId);

                if (player is null)
                {
                    return NotFound();
                }

                inventory = Inventory.CreateStorage(playerId, request.Slots);
            }
            else
            {
                inventory.Slots = request.Slots;
            }

            inventoryRepository.AddOrUpdate(inventory);

            await DataContextManager.SaveAsync();

            logger.LogInformation("Storage {inventoryId} updated", inventory.Id);
            return inventory;
        }

        [HttpPut("player/{playerId}/equipment")]
        public async Task<ActionResult<Inventory>> UpdateEquipment(string playerId,
            [FromBody] InventoryUpdateSlotsModel request)
        {
            using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = DataContextManager.CreateRepository<IInventoryRepository>();

            var inventory = await inventoryRepository.GetEquipment(playerId);

            if (inventory == null)
            {
                var playerRepository = DataContextManager.CreateRepository<IPlayerRepository>();
                var player = await playerRepository.GetByIdAsync(playerId);

                if (player is null)
                {
                    return NotFound();
                }

                inventory = Inventory.CreateEquipment(playerId, request.Slots);
            }
            else
            {
                inventory.Slots = request.Slots;
            }

            inventoryRepository.AddOrUpdate(inventory);

            await DataContextManager.SaveAsync();

            logger.LogInformation("Equipment {inventoryId} updated", inventory.Id);
            return inventory;
        }
    }
}