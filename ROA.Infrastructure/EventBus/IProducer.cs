using Confluent.Kafka;

namespace ROA.Infrastructure.EventBus;

public interface IProducer<TKey, TValue>
{
    Task<DeliveryResult<TKey,TValue>> ProduceAsync(TKey key, TValue message);
    Task<DeliveryResult<TKey,TValue>> ProduceAsync(TValue message);
    void Produce(TKey key, TValue message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null);
    void Produce(TValue message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null);
    void Flush(TimeSpan timeout);
}