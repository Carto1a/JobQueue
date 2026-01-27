using System.Text;
using System.Text.Json;
using JobQueue.Application;
using RabbitMQ.Client;

namespace JobQueue.Infrastructure.Messaging;

public class QueuePublisher(IConnection connection) : IQueuePublisher
{
    private readonly IConnection _connection = connection;

    public async Task PublishJob(Guid id)
    {
        const string queueName = "jobs";

        using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            null);

        var message = JsonSerializer.Serialize(new { Id = id });
        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            "",
            queueName,
            mandatory: false,
            props,
            body
        );
    }
}
