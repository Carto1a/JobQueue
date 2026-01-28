using JobQueue.Domain;
using JobQueue.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using JobQueue.Application;
using JobQueue.Infrastructure.Processors;
using RabbitMQ.Client;
using JobQueue.Infrastructure.Messaging;
using MongoDB.Driver;

namespace JobQueue.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection InjectInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<MongoDbSettings>()
            .Bind(configuration.GetSection("MongoDb"))
            .Validate(s =>
                !string.IsNullOrWhiteSpace(s.ConnectionString),
                "MongoDb:ConnectionString is required")
            .Validate(s =>
                !string.IsNullOrWhiteSpace(s.DatabaseName),
                "MongoDb:DatabaseName is required")
            .ValidateOnStart();

        services.AddOptions<RabbitMqSettings>()
            .Bind(configuration.GetSection("RabbitMq"))
            .Validate(s =>
                !string.IsNullOrWhiteSpace(s.Host),
                "RabbitMq:Host is required")
            .Validate(s => s.Port > 0 && s.Port <= 65535,
                "RabbitMq:Port is invalid")
            .Validate(s =>
                !string.IsNullOrWhiteSpace(s.Username),
                "RabbitMq:Username is required")
            .Validate(s =>
                !string.IsNullOrWhiteSpace(s.Password),
                "RabbitMq:Password is required")
            .Validate(s => s.DispatchConcurrency > 0,
                "RabbitMq:DispatchConcurrency must be greater than 0")
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<MongoClient>();
            return new MongoDbContext(client, settings);
        });

        services.AddSingleton<MongoInitializer>();

        services.AddScoped<IJobRepository, JobRepository>();

        services.AddScoped<IJobProcessor, EmailJobProcessor>();
        services.AddScoped<IJobProcessor, ReportJobProcessor>();

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

            var factory = new ConnectionFactory
            {
                HostName = settings.Host,
                Port = settings.Port,
                UserName = settings.Username,
                Password = settings.Password,
                ConsumerDispatchConcurrency = settings.DispatchConcurrency
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddScoped<IQueuePublisher, QueuePublisher>();
        services.AddScoped<IQueueConsumer, QueueConsumer>();

        return services;
    }
}
