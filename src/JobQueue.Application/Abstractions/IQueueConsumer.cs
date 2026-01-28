namespace JobQueue.Application.Abstractions;

public interface IQueueConsumer
{
    Task ConsumeJobs(IProcessJobHandler handler, CancellationToken cancellationToken = default);
}
