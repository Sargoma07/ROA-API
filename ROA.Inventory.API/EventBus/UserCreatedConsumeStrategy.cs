using Confluent.Kafka;
using ROA.Domain.Events;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.EventBus.Kafka;
using ROA.Inventory.API.Data.Repositories;
using ROA.Inventory.API.Domain.Types;

namespace ROA.Inventory.API.EventBus;

public class UserCreatedConsumeStrategy(
    IDataContextManager dataContextManager,
    ILogger<UserCreatedConsumeStrategy> logger
)
    : IConsumeStrategy<Null, UserCreatedEvent>
{
    public async Task<ConsumeResult<Null, UserCreatedEvent>> ConsumeProcessAsync(
        ConsumeResult<Null, UserCreatedEvent> result, CancellationToken cancellationToken)
    {
        var message = result.Message.Value;

        if (message is null)
        {
            logger.LogWarning("Message is null");
            return result;
        }

        await InitInventoryForNewPlayer(message);

        return result;
    }
    
    private async Task InitInventoryForNewPlayer(UserCreatedEvent message)
    {
        await CreateInventory(message);
        await CreateEquipment(message);
        await CreateStorage(message);
    }

    private async Task CreateStorage(UserCreatedEvent message)
    {
        var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();

        var playerId = message.UserId;

        var storage = await inventoryRepository.GetStorage(playerId);

        using var _ = logger.BeginScope(new Dictionary<string, string>()
        {
            { "PlayerId", playerId }
        });

        if (storage is not null)
        {
            logger.LogWarning("Storage {StorageId} already exists for user", storage.Id);
            return;
        }

        storage = new Domain.Inventory
        {
            PlayerId = message.UserId,
            Type = InventoryType.Storage
        };

        inventoryRepository.AddOrUpdate(storage);

        await dataContextManager.SaveAsync();

        logger.LogInformation("Storage {StorageId} created", storage.Id);
    }

    private async Task CreateEquipment(UserCreatedEvent message)
    {
        var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();

        var playerId = message.UserId;

        var equipment = await inventoryRepository.GetEquipment(playerId);

        using var _ = logger.BeginScope(new Dictionary<string, string>()
        {
            { "PlayerId", playerId }
        });

        if (equipment is not null)
        {
            logger.LogWarning("Equipment {EquipmentId} already exists for user", equipment.Id);
            return;
        }

        equipment = new Domain.Inventory
        {
            PlayerId = message.UserId,
            Type = InventoryType.Equipment
        };

        inventoryRepository.AddOrUpdate(equipment);

        await dataContextManager.SaveAsync();

        logger.LogInformation("Equipment {EquipmentId} created", equipment.Id);
    }

    private async Task CreateInventory(UserCreatedEvent message)
    {
        var inventoryRepository = dataContextManager.CreateRepository<IInventoryRepository>();

        var playerId = message.UserId;

        var inventory = await inventoryRepository.GetInventory(playerId);

        using var _ = logger.BeginScope(new Dictionary<string, string>()
        {
            { "PlayerId", playerId }
        });

        if (inventory is not null)
        {
            logger.LogWarning("Inventory {InventoryId} already exists for user", inventory.Id);
            return;
        }

        inventory = new Domain.Inventory
        {
            PlayerId = message.UserId,
            Type = InventoryType.CharacterInventory
        };

        inventoryRepository.AddOrUpdate(inventory);

        await dataContextManager.SaveAsync();

        logger.LogInformation("Inventory {InventoryId} created", inventory.Id);
    }
}