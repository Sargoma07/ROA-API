using Confluent.Kafka;

namespace ROA.Infrastructure.EventBus;

public interface IConsumer<TKey, TValue> : IDisposable
{
    void Subscribe();
    void Unsubscribe();
    Task<ConsumeResult<TKey, TValue>?> ConsumeAsync(CancellationToken cancellationToken);
    void Close();
}