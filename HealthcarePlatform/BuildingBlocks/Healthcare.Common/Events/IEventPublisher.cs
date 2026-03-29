namespace Healthcare.Common.Events;

/// <summary>
/// Abstraction for future Kafka/RabbitMQ integration. No transport implementation in this template.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<T>(string topicOrExchange, T message, CancellationToken cancellationToken = default)
        where T : class;
}
