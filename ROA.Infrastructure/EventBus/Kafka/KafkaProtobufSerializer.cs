using Confluent.Kafka;
using Google.Protobuf;

namespace ROA.Infrastructure.EventBus.Kafka;

public class KafkaProtobufSerializer<T> : ISerializer<T> where T : IMessage
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        using var stream = new MemoryStream();
        data.WriteTo(stream);
        return stream.ToArray();
    }
}