using Confluent.Kafka;
using Google.Protobuf;

namespace ROA.Infrastructure.EventBus.Kafka;

internal class ErrorKafkaProducer<TKey, TValue> : KafkaProducer<TKey, TValue> where TValue : IMessage
{
    public ErrorKafkaProducer(string errorTopic, KafkaClientHandle handle, ISerializer<TValue> serializer)
        : base(handle, serializer)
    {
        Topic = errorTopic;
    }
}