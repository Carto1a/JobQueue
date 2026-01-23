namespace JobQueue.Application;

public interface IQueuePublisher
{
    Task Publish(Guid id);
}
