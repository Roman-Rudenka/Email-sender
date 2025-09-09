namespace Domain.Models;

public class OutboxMessage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string? Payload { get; set; }
    public required string QueueName { get; init; }
    public DateTime CreatedAt { get; init; }
    public required OutBoxStatus Status { get; set; } = OutBoxStatus.Pending;

    public enum OutBoxStatus
    {
        Pending, Published, Failed
    }
}