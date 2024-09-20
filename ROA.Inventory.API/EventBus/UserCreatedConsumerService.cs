using Confluent.Kafka;
using ROA.Domain.Events;
using ROA.Infrastructure.EventBus;

namespace ROA.Inventory.API.EventBus;


public class UserCreatedConsumerService(IUserCreatedConsumer consumer, ILogger<UserCreatedConsumerService> logger)
    : ConsumerBackgroundService<Null, UserCreatedEvent>(consumer, logger)
{
}