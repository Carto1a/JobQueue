namespace JobQueue.Application;

public interface IQueueConsumer
{
    Task ConsumeJobs(CancellationToken cancellationToken = default);
}
