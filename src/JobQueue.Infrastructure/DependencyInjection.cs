using JobQueue.Domain;
using JobQueue.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using JobQueue.Application;
using JobQueue.Infrastructure.Processors;

namespace JobQueue.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection InjectInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("MongoDb:ConnectionString not configured");

            return new MongoDbContext(settings);
        });

        services.AddScoped<IJobRepository, JobRepository>();

        services.AddScoped<IJobProcessor, EmailJobProcessor>();
        services.AddScoped<IJobProcessor, ReportJobProcessor>();

        return services;
    }
}
