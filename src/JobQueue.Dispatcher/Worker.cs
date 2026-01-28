using JobQueue.Application;

namespace JobQueue.Dispatcher;

public class Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Job dispatcher started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var publisher = scope.ServiceProvider.GetRequiredService<IQueuePublisher>();

                if (!await publisher.IsHealthy(stoppingToken))
                {
                    logger.LogWarning("RabbitMQ unavailable. Dispatcher paused.");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                var handler = scope.ServiceProvider
                    .GetRequiredService<DispatchPendingJobsHandler>();

                await handler.Handle(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Dispatcher stopping gracefully");
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Fatal error in dispatcher loop");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
