using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OutboxMessageRepository(AppDbContext context) : IOutboxMessageRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        await _context.AddAsync(outboxMessage, cancellationToken);
    }

    public async Task<List<OutboxMessage>> GetAllUnpublishedMessages(CancellationToken cancellationToken)
    {
        return await _context.OutboxMessages.Where(o => o.Status == OutboxMessage.OutBoxStatus.Pending).OrderBy(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void MarkAsPublished(OutboxMessage outboxMessage)
    {
        outboxMessage.Status = OutboxMessage.OutBoxStatus.Published;
    }
    public void MarkAsFailed(OutboxMessage outboxMessage)
    {
        outboxMessage.Status = outboxMessage.Status = OutboxMessage.OutBoxStatus.Failed;
    }
}