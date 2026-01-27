using MongoDB.Driver;

namespace JobQueue.Infrastructure.Persistence;

public record MongoDbSettings(string ConnectionString, string DatabaseName);

public class MongoDbContext(MongoClient client, MongoDbSettings settings)
{
    private readonly IMongoDatabase _database = client.GetDatabase(settings.DatabaseName);

    public IMongoCollection<JobModel> Jobs => _database.GetCollection<JobModel>("jobs");
}
