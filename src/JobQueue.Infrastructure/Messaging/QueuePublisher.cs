using System.Text;
using JobQueue.Application;
using RabbitMQ.Client;

namespace JobQueue.Infrastructure.Messaging;

public class QueuePublisher(IRabbitMqChannelFactory factory) : IQueuePublisher
{
    private readonly IRabbitMqChannelFactory _factory = factory;

    public async Task PublishJob(Guid id)
    {
        using var channel = await _factory.CreateConsumerChannel(QueueName.Jobs);

        var body = Encoding.UTF8.GetBytes(id.ToString());

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "text/plain"
        };

        await channel.BasicPublishAsync(
            "",
            QueueName.Jobs,
            mandatory: false,
            props,
            body
        );
    }
}
