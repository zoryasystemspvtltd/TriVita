namespace Healthcare.Common.Events;

/// <summary>Placeholder publisher for local/dev until messaging is wired.</summary>
public sealed class NoOpEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(string topicOrExchange, T message, CancellationToken cancellationToken = default)
        where T : class =>
        Task.CompletedTask;
}
