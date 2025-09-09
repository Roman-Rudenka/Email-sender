using Domain.Models;

namespace Application.Interfaces;

public interface IOutboxMessageRepository
{
    public Task AddMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken);
    public Task<List<OutboxMessage>> GetAllUnpublishedMessages(CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public void MarkAsPublished(OutboxMessage outboxMessage);
    public void MarkAsFailed(OutboxMessage outboxMessage);
}