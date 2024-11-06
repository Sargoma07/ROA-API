namespace ROA.Payment.API.Settings;

public record TopicSettings
{
    public required string UserCreatedTopic { get; init; }
    public required string UserCreatedTopicError { get; init; }
}