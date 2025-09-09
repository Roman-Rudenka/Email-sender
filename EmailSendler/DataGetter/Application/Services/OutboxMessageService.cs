using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class OutboxMessageService(IOutboxMessageRepository repository, IRabbitMqService rabbitMq)
    : IOutboxMessageService
{
    public async Task EnqueueAsync(string payload, string queueName, CancellationToken cancellationToken)
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Payload = payload,
            QueueName = queueName,
            Status = OutboxMessage.OutBoxStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddMessageAsync(message, cancellationToken);
    }

    public async Task PendingMessagesAsync(CancellationToken cancellationToken)
    {
        var messages = await repository.GetAllUnpublishedMessages(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                rabbitMq.SendEmail(message.Payload ?? string.Empty);
                repository.MarkAsPublished(message);
            }
            catch (Exception)
            {
                repository.MarkAsFailed(message);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
    }
}