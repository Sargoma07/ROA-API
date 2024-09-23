using Microsoft.Extensions.Options;
using ROA.Domain.Events;
using ROA.Infrastructure.EventBus.Kafka;
using ROA.Payment.API.Settings;
using Null = Confluent.Kafka.Null;

namespace ROA.Payment.API.EventBus;

public class UserCreatedConsumer : KafkaConsumer<Null, UserCreatedEvent>
{
    public UserCreatedConsumer(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<TopicSettings> settings,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<UserCreatedConsumer> logger
    ) :
        base(
            serviceScopeFactory,
            typeof(UserCreatedConsumeStrategy),
            kafkaSettings,
            new KafkaProtobufDeserializer<UserCreatedEvent>(),
            logger
        )
    {
        Topic = settings.Value.UserCreatedTopic;
        ErrorTopic = settings.Value.UserCreatedTopicError;
    }
}