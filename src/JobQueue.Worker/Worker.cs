using JobQueue.Infrastructure.Messaging;

namespace JobQueue.Worker;

public class Worker(QueueConsumer consumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.ConsumeJobs(stoppingToken);
    }
}
