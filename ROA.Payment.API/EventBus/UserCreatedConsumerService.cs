using Confluent.Kafka;
using ROA.Domain.Events;
using ROA.Infrastructure.EventBus;

namespace ROA.Payment.API.EventBus;


public class UserCreatedConsumerService(UserCreatedConsumer consumer, ILogger<UserCreatedConsumerService> logger)
    : ConsumerBackgroundService<Null, UserCreatedEvent>(consumer, logger)
{
}