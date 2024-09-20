using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ROA.Infrastructure.EventBus;

public abstract class ConsumerBackgroundService<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    ILogger logger)
    : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartConsumerLoop(stoppingToken);
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        consumer.Subscribe();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await consumer.ConsumeAsync(cancellationToken);
            }
            catch (ConsumeException e)
            {
                logger.LogError(e, "Consume error: {reason}", e.Error.Reason);
                
                if (e.Error.IsFatal)
                {
                    // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    break;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unhandled exception");
                throw;
            }
        }
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}