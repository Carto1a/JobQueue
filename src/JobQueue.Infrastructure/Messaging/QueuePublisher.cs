using System.Text;
using JobQueue.Application;
using RabbitMQ.Client;

namespace JobQueue.Infrastructure.Messaging;

public class QueuePublisher(IRabbitMqChannelFactory factory, IConnection connection) : IQueuePublisher
{
    private readonly IRabbitMqChannelFactory _factory = factory;
    private readonly IConnection _connection = connection;

    public async Task<bool> IsHealthy(CancellationToken cancellationToken = default)
    {
        try
        {
            using var channel = await _connection.CreateChannelAsync();
            return channel.IsOpen;
        }
        catch
        {
            return false;
        }
    }

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
