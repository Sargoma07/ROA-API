using Confluent.Kafka;
using Google.Protobuf;

namespace ROA.Infrastructure.EventBus.Kafka;

public class KafkaProtobufDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty)
        {
            return new T();
        }

        var prop = typeof(T).GetProperty("Parser");
        
        if (prop is null)
        {
            return new T();
        }

        if (prop.GetValue(null, null) is not MessageParser<T> parser)
        {
            return new T();
        }

        var bytes = new MemoryStream(data.ToArray());
        return parser.ParseFrom(bytes);
    }
}