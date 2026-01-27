using JobQueue.Application;

namespace JobQueue.Worker;

public class Worker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IProcessJobHandler>();
        var consumer = scope.ServiceProvider.GetRequiredService<IQueueConsumer>();

        await consumer.ConsumeJobs(handler, stoppingToken);
    }
}
