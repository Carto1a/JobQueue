using MongoDB.Driver;

namespace JobQueue.Infrastructure.Persistence;

public record MongoDbSettings(string ConnectionString, string DatabaseName);

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<JobModel> Jobs => _database.GetCollection<JobModel>("jobs");

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }
}
