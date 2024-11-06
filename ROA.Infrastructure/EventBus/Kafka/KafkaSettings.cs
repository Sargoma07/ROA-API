namespace ROA.Infrastructure.EventBus.Kafka;

public record KafkaSettings
{
    public required ProducerSettings Producer { get; init; }
    
    public required ConsumerSettings Consumer { get; init; }

    public record ProducerSettings
    {
        public required string BootstrapServers { get; init; }
    }
    
    public record ConsumerSettings
    {
        public required string BootstrapServers { get; init; }
        public required string GroupId { get; init; }
    }
}