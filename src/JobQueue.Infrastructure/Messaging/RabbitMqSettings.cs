namespace JobQueue.Infrastructure.Messaging;

public record RabbitMqSettings(
    string Host,
    int Port,
    string QueueName,
    string Username,
    string Password);
