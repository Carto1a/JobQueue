namespace JobQueue.Application.Abstractions;

public interface IQueuePublisher
{
    Task PublishJob(Guid id);
    Task<bool> IsHealthy(CancellationToken cancellationToken = default);
}
