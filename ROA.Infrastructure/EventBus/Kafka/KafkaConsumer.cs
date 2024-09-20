using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ROA.Infrastructure.EventBus.Kafka;

public abstract class KafkaConsumer<TKey, TValue> : IConsumer<TKey, TValue>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Type _consumerStrategyType;
    private readonly ILogger _logger;
    private readonly Confluent.Kafka.IConsumer<TKey, TValue> _kafkaConsumer;

    protected string Topic { get; init; } = string.Empty;

    protected KafkaConsumer(
        IServiceScopeFactory serviceScopeFactory,
        Type consumerStrategyType,
        IOptions<KafkaSettings> settings,
        IDeserializer<TValue> deserializer,
        ILogger logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _consumerStrategyType = consumerStrategyType;
        _logger = logger;
        var consumerSettings = settings.Value.Consumer;

        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = consumerSettings.BootstrapServers,
            GroupId = consumerSettings.GroupId
        };

        _kafkaConsumer = new ConsumerBuilder<TKey, TValue>(consumerConfig)
            .SetValueDeserializer(deserializer)
            .Build();
    }

    public void Subscribe()
    {
        _kafkaConsumer.Subscribe(Topic);
    }

    public void Unsubscribe()
    {
        _kafkaConsumer.Unsubscribe();
    }

    public async Task<ConsumeResult<TKey, TValue>?> ConsumeAsync(CancellationToken cancellationToken)
    {
        return await _kafkaConsumer.ConsumeWithInstrumentation(HandleConsumeProcessAsync, cancellationToken);
    }

    public void Close()
    {
        _kafkaConsumer.Close();
    }

    public void Dispose()
    {
        Close();
    }

    private async Task<ConsumeResult<TKey, TValue>?> HandleConsumeProcessAsync(ConsumeResult<TKey, TValue>? result,
        CancellationToken cancellationToken)
    {
        try
        {
            if (result is null)
            {
                throw new InvalidOperationException($"Result of consuming of topic {Topic} is null");
            }

            using var _ = _logger.BeginScope(new Dictionary<string, string> { { "Offset", result.Offset.ToString() } });

            _logger.LogInformation("Starting to consume topic: {Topic}", Topic);
            using var scope = _serviceScopeFactory.CreateScope();

            if (scope.ServiceProvider.GetRequiredService(_consumerStrategyType) is not
                IConsumeStrategy<TKey, TValue> strategy)
            {
                throw new InvalidOperationException($"Invalid strategy for {GetType().Name}");
            }

            var consumeResult = await strategy.ConsumeProcessAsync(result, cancellationToken);

            _logger.LogInformation("Executed consuming of topic: {Topic}", Topic);

            return consumeResult;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception");
        }

        return result;
    }
}