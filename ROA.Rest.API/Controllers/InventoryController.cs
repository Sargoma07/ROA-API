using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;
using ROA.Rest.API.Models;

namespace ROA.Rest.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IDataContextManager dataContextManager, ILogger<InventoryController> logger)
        : ControllerBase
    {
        [HttpGet("player/{playerId}/inventory")]
        public async Task<ActionResult<Inventory>> GetInventory(string playerId)
        {
            var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();
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
            var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();
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
            var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetEquipment(playerId);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        [HttpPut("player/{playerId}/inventory")]
        public async Task<ActionResult<Inventory>> UpdateInventory(string playerId,
            [FromBody] InventoryUpdateSlots request)
        {
            using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetInventory(playerId);

            if (inventory == null)
            {
                var playerRepository = dataContextManager.CreateRepository<IPlayerRepository>();
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

            await dataContextManager.SaveAsync();

            logger.LogInformation("Inventory {inventoryId} updated", inventory.Id);
            return inventory;
        }

        [HttpPut("player/{playerId}/storage")]
        public async Task<ActionResult<Inventory>> UpdateStorage(string playerId,
            [FromBody] InventoryUpdateSlots request)
        {
            using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();
            var inventory = await inventoryRepository.GetStorage(playerId);

            if (inventory == null)
            {
                var playerRepository = dataContextManager.CreateRepository<IPlayerRepository>();
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

            await dataContextManager.SaveAsync();

            logger.LogInformation("Storage {inventoryId} updated", inventory.Id);
            return inventory;
        }

        [HttpPut("player/{playerId}/equipment")]
        public async Task<ActionResult<Inventory>> UpdateEquipment(string playerId,
            [FromBody] InventoryUpdateSlots request)
        {
            using var _ = logger.BeginScope(new Dictionary<string, object> { { "PlayerId", playerId } });

            var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();

            var inventory = await inventoryRepository.GetEquipment(playerId);

            if (inventory == null)
            {
                var playerRepository = dataContextManager.CreateRepository<IPlayerRepository>();
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

            await dataContextManager.SaveAsync();

            logger.LogInformation("Equipment {inventoryId} updated", inventory.Id);
            return inventory;
        }

        // DELETE api/<Inventory>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}