namespace JobQueue.Application;

public interface IQueuePublisher
{
    Task PublishJob(Guid id);
    Task<bool> IsHealthy(CancellationToken cancellationToken = default);
}
