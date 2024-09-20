using Microsoft.Extensions.Options;
using ROA.Domain.Events;
using ROA.Identity.API.Settings;
using ROA.Infrastructure.EventBus.Kafka;
using Null = Confluent.Kafka.Null;

namespace ROA.Identity.API.EventBus;

public interface IUserCreatedProducer : Infrastructure.EventBus.IProducer<Null, UserCreatedEvent>
{
}

// TODO: extract serialization from KafkaProducer. It should configure into Program.cs and for all producers
public class UserCreatedProducer : KafkaProducer<Null, UserCreatedEvent>, IUserCreatedProducer
{
    public UserCreatedProducer(IOptions<TopicSettings> settings, KafkaClientHandle handle) : base(handle,
        new KafkaProtobufSerializer<UserCreatedEvent>())
    {
        Topic = settings.Value.UserCreatedTopic;
    }
}