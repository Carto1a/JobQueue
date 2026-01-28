using System.Text;
using JobQueue.Application;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JobQueue.Infrastructure.Messaging;

public class QueueConsumer(
    IRabbitMqChannelFactory factory,
    ILogger<QueueConsumer> logger
) : IQueueConsumer
{
    private readonly IRabbitMqChannelFactory _factory = factory;
    private readonly ILogger<QueueConsumer> _logger = logger;

    public async Task ConsumeJobs(IProcessJobHandler handler, CancellationToken cancellationToken = default)
    {
        var channel = await _factory.CreateConsumerChannel(QueueName.Jobs);

        try
        {
            await SetupConsumer(handler, channel, cancellationToken);
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

    private async Task SetupConsumer(IProcessJobHandler handler, IChannel channel, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                return;
            }

            var rawMessage = Encoding.UTF8.GetString(ea.Body.Span);

            try
            {
                if (!Guid.TryParse(rawMessage, out var id))
                {
                    _logger.LogError(
                        "Invalid message format. Expected GUID but received '{Payload}'. " +
                        "DeliveryTag={DeliveryTag}, Redelivered={Redelivered}, Exchange={Exchange}, RoutingKey={RoutingKey}",
                        rawMessage,
                        ea.DeliveryTag,
                        ea.Redelivered,
                        ea.Exchange,
                        ea.RoutingKey
                    );

                    await channel.BasicNackAsync(
                        ea.DeliveryTag,
                        multiple: false,
                        requeue: false);
                    return;
                }

                var result = await handler.Handle(id, cancellationToken);
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
            QueueName.Jobs,
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
