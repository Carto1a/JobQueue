using JobQueue.Infrastructure.Persistence;

namespace JobQueue.Api;

public static class ApplicationBuilderExtensions
{
    public static async Task InitializeDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<MongoInitializer>();

        await initializer.Init();
    }
}
