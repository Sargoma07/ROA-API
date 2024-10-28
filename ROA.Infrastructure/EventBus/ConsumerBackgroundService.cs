using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ROA.Infrastructure.EventBus;

public abstract class ConsumerBackgroundService<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    ILogger logger)
    : BackgroundService
{
    
    protected override Task<Task> ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.FromResult(Task.Run(async () =>await StartConsumerLoop(stoppingToken), stoppingToken));
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting consumer background service {name}", GetType().Name);

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
        
        logger.LogInformation("Stopped consumer background service {name}", GetType().Name);
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}