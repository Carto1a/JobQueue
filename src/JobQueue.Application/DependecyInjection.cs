using Microsoft.Extensions.DependencyInjection;

namespace JobQueue.Application;

public static class DependecyInjection
{
    public static IServiceCollection InjectUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateJobHandler>();
        services.AddScoped<DispatchPendingJobsHandler>();
        services.AddScoped<IProcessJobHandler, ProcessJobHandler>();
        services.AddScoped<GetJobQueryHandler>();
        services.AddScoped<GetJobsQueryHandler>();

        return services;
    }
}
