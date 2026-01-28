using System.Text;
using System.Text.Json;
using JobQueue.Application;
using RabbitMQ.Client;

namespace JobQueue.Infrastructure.Messaging;

public class QueuePublisher(IRabbitMqChannelFactory factory) : IQueuePublisher
{
    private readonly IRabbitMqChannelFactory _factory = factory;

    public async Task PublishJob(Guid id)
    {
        using var channel = await _factory.CreateConsumerChannel(QueueName.Jobs);

        var message = JsonSerializer.Serialize(new { Id = id });
        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            "",
            QueueName.Jobs,
            mandatory: false,
            props,
            body
        );
    }
}
