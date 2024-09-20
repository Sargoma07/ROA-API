using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;

namespace ROA.Infrastructure.EventBus.Kafka;

public abstract class KafkaProducer<TKey, TValue> : IProducer<TKey, TValue>
{
    private readonly Confluent.Kafka.IProducer<TKey, TValue> _kafkaProducer;

    protected KafkaProducer(KafkaClientHandle handle, ISerializer<TValue> serializer)
    {
        _kafkaProducer = new DependentProducerBuilder<TKey, TValue>(handle.Handle)
            .SetValueSerializer(serializer)
            .BuildWithInstrumentation();
    }

    protected string Topic { get; init; } = string.Empty;


    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(TKey key, TValue message)
    {
        return await ProduceAsync(new Message<TKey, TValue> { Key = key, Value = message });
    }

    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(TValue message)
    {
        return await ProduceAsync(new Message<TKey, TValue> { Value = message });
    }

    public void Produce(TKey key, TValue message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        Produce(new Message<TKey, TValue> { Key = key, Value = message }, deliveryHandler);
    }

    public void Produce(TValue message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        Produce(new Message<TKey, TValue> { Value = message }, deliveryHandler);
    }

    public void Flush(TimeSpan timeout)
    {
        _kafkaProducer.Flush(timeout);
    }

    private void Produce(Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        _kafkaProducer.Produce(Topic, message, deliveryHandler);
    }

    private async Task<DeliveryResult<TKey, TValue>> ProduceAsync(Message<TKey, TValue> message)
    {
        return await _kafkaProducer.ProduceAsync(Topic, message);
    }
}