namespace ROA.Infrastructure.EventBus.Kafka;

public class KafkaSettings
{
    public required ProducerSettings Producer { get; set; }
    
    public required ConsumerSettings Consumer { get; set; }

    public record ProducerSettings
    {
        public required string BootstrapServers { get; set; }
    }
    
    public record ConsumerSettings
    {
        public required string BootstrapServers { get; set; }
        public required string GroupId { get; set; }
    }
}