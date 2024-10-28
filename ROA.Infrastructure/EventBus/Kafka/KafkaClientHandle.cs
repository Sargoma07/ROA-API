using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace ROA.Infrastructure.EventBus.Kafka;

public class KafkaClientHandle : IDisposable
{
    private readonly Confluent.Kafka.IProducer<byte[], byte[]> _kafkaProducer;

    public KafkaClientHandle(IOptions<KafkaSettings> settings)
    {
        var producerSettings = settings.Value.Producer;

        var conf = new ProducerConfig()
        {
            BootstrapServers = producerSettings.BootstrapServers
        };

        _kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();
    }

    public Handle Handle => _kafkaProducer.Handle;

    public void Dispose()
    {
        _kafkaProducer.Flush();
        _kafkaProducer.Dispose();
    }
}