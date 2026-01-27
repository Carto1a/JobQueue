using JobQueue.Application;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JobQueue.Infrastructure.Messaging;

public class QueueConsumer(
    IConnection connection,
    ILogger<QueueConsumer> logger,
    ProcessJobHandler handler
) : IQueueConsumer
{
    private readonly IConnection _connection = connection;
    private readonly ILogger<QueueConsumer> _logger = logger;
    private readonly ProcessJobHandler _handler = handler;

    public async Task ConsumeJobs(CancellationToken cancellationToken = default)
    {
        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        try
        {
            await SetupConsumer(channel, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Queue consumer shutting down");
        }
        finally
        {
            if (channel.IsOpen)
                await channel.CloseAsync(CancellationToken.None);

            await channel.DisposeAsync();
        }
    }

    private async Task SetupConsumer(IChannel channel, CancellationToken cancellationToken)
    {
        const string queueName = "jobs";

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                return;
            }

            try
            {
                if (!Guid.TryParse(ea.Body.ToArray(), out var id))
                {
                    _logger.LogError(
                        "Failed to process message. DeliveryTag={DeliveryTag}, Redelivered={Redelivered}",
                        ea.DeliveryTag,
                        ea.Redelivered
                    );

                    await channel.BasicNackAsync(
                        ea.DeliveryTag,
                        multiple: false,
                        requeue: false);
                    return;
                }

                var result = await _handler.Handle(id, cancellationToken);
                switch (result)
                {
                    case ProcessResult.Completed:
                    case ProcessResult.NotFound:
                        await channel.BasicAckAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            cancellationToken);
                        break;
                    case ProcessResult.Retry:
                        await channel.BasicNackAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            requeue: true);
                        break;
                    case ProcessResult.Failed:
                        await channel.BasicNackAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            requeue: false);
                        break;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process message");

                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    requeue: false);
            }
        };

        var consumerTag = await channel.BasicConsumeAsync(
            queueName,
            autoAck: false,
            consumer,
            cancellationToken);

        cancellationToken.Register(async () =>
        {
            _logger.LogInformation("Cancelling RabbitMQ consumer");
            await channel.BasicCancelAsync(consumerTag);
        });

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
