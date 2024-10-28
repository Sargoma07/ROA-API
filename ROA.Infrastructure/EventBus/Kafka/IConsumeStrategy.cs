using Confluent.Kafka;

namespace ROA.Infrastructure.EventBus.Kafka;

public interface IConsumeStrategy<TKey, TValue>
{
    Task<ConsumeResult<TKey, TValue>> ConsumeProcessAsync(
        ConsumeResult<TKey, TValue> result, CancellationToken cancellationToken);
}