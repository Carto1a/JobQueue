using RabbitMQ.Client;

namespace JobQueue.Infrastructure.Messaging;

public interface IRabbitMqChannelFactory
{
    Task<IChannel> CreateConsumerChannel(string queueName, ushort prefetch = 1);
}

public class RabbitMqChannelFactory(IConnection connection) : IRabbitMqChannelFactory
{
    private readonly IConnection _connection = connection;

    public async Task<IChannel> CreateConsumerChannel(string queueName, ushort prefetch = 1)
    {
        var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: prefetch,
            global: false);

        return channel;
    }
}
