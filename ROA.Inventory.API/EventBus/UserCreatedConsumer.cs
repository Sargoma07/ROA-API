using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ROA.Domain.Events;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.EventBus.Kafka;
using ROA.Inventory.API.Data.Repositories;
using ROA.Inventory.API.Domain.Types;
using ROA.Inventory.API.Settings;
using Null = Confluent.Kafka.Null;

namespace ROA.Inventory.API.EventBus;

public interface IUserCreatedConsumer : Infrastructure.EventBus.IConsumer<Null, UserCreatedEvent>
{
}

public class UserCreatedConsumer : KafkaConsumer<Null, UserCreatedEvent>, IUserCreatedConsumer
{
    private readonly IDataContextManager _dataContextManager;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(
        IDataContextManager dataContextManager,
        IOptions<TopicSettings> settings,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<UserCreatedConsumer> logger
    ) :
        base(kafkaSettings,
            new KafkaProtobufDeserializer<UserCreatedEvent>(),
            logger
        )
    {
        _dataContextManager = dataContextManager;
        _logger = logger;
        Topic = settings.Value.UserCreatedTopic;
    }

    protected override async Task<ConsumeResult<Null, UserCreatedEvent>> ConsumeProcessAsync(
        ConsumeResult<Null, UserCreatedEvent> result, CancellationToken cancellationToken)
    {
        var message = result.Message.Value;

        if (message is null)
        {
            _logger.LogWarning("Message is null");
        }
        else
        {
            await InitInventoryForNewPlayer(message);
        }

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
        var inventoryRepository = _dataContextManager.CreateRepository<IInventoryRepository>();

        var playerId = message.UserId;

        var storage = await inventoryRepository.GetStorage(playerId);

        using var _ = _logger.BeginScope(new Dictionary<string, string>()
        {
            { "PlayerId", playerId }
        });

        if (storage is not null)
        {
            _logger.LogWarning("Storage {StorageId} already exists for user", storage.Id);
            return;
        }

        storage = new Domain.Inventory
        {
            PlayerId = message.UserId,
            Type = InventoryType.Storage
        };

        inventoryRepository.AddOrUpdate(storage);

        await _dataContextManager.SaveAsync();

        _logger.LogInformation("Storage {StorageId} created", storage.Id);
    }

    private async Task CreateEquipment(UserCreatedEvent message)
    {
        var inventoryRepository = _dataContextManager.CreateRepository<IInventoryRepository>();

        var playerId = message.UserId;

        var equipment = await inventoryRepository.GetEquipment(playerId);

        using var _ = _logger.BeginScope(new Dictionary<string, string>()
        {
            { "PlayerId", playerId }
        });

        if (equipment is not null)
        {
            _logger.LogWarning("Equipment {EquipmentId} already exists for user", equipment.Id);
            return;
        }

        equipment = new Domain.Inventory
        {
            PlayerId = message.UserId,
            Type = InventoryType.Equipment
        };

        inventoryRepository.AddOrUpdate(equipment);

        await _dataContextManager.SaveAsync();

        _logger.LogInformation("Equipment {EquipmentId} created", equipment.Id);
    }

    private async Task CreateInventory(UserCreatedEvent message)
    {
        var inventoryRepository = _dataContextManager.CreateRepository<IInventoryRepository>();

        var playerId = message.UserId;

        var inventory = await inventoryRepository.GetInventory(playerId);

        using var _ = _logger.BeginScope(new Dictionary<string, string>()
        {
            { "PlayerId", playerId }
        });

        if (inventory is not null)
        {
            _logger.LogWarning("Inventory {InventoryId} already exists for user", inventory.Id);
            return;
        }

        inventory = new Domain.Inventory
        {
            PlayerId = message.UserId,
            Type = InventoryType.CharacterInventory
        };

        inventoryRepository.AddOrUpdate(inventory);

        await _dataContextManager.SaveAsync();

        _logger.LogInformation("Inventory {InventoryId} created", inventory.Id);
    }
}