namespace JobQueue.Application;

public interface IQueuePublisher
{
    Task PublishJob(Guid id);
}
