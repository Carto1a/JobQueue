using MongoDB.Driver;

namespace JobQueue.Infrastructure.Persistence;

public class MongoInitializer(MongoClient client, MongoDbSettings settings)
{
    private const string JobsCollectionName = "jobs";

    private readonly IMongoDatabase _database = client.GetDatabase(settings.DatabaseName);

    public async Task Init()
    {
        await CreateJobsCollection();
    }

    private async Task CreateJobsCollection()
    {
        var collection = await _database.ListCollectionNames().ToListAsync();

        if (!collection.Contains(JobsCollectionName))
            await _database.CreateCollectionAsync(JobsCollectionName);

        var jobs = _database.GetCollection<JobModel>(JobsCollectionName);

        var indexs = new[]
        {
            new CreateIndexModel<JobModel>(
                Builders<JobModel>.IndexKeys.Ascending(x => x.Status)),
            new CreateIndexModel<JobModel>(
                Builders<JobModel>.IndexKeys.Descending(x => x.CreatedAt))
        };

        await jobs.Indexes.CreateManyAsync(indexs);
    }
}
