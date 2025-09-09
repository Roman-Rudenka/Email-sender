namespace Application.Interfaces;

public interface IOutboxMessageService
{
    public Task PendingMessagesAsync(CancellationToken cancellationToken);
    public Task EnqueueAsync(string payload, string queueName, CancellationToken cancellationToken);
}