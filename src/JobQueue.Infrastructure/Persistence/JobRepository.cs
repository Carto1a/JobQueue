using JobQueue.Domain;
using MongoDB.Driver;

namespace JobQueue.Infrastructure.Persistence;

public class JobRepository(MongoDbContext context) : IJobRepository
{
    private readonly IMongoCollection<JobModel> _collection = context.Jobs;

    public async Task Create(Job job, CancellationToken cancellationToken = default)
    {
        var document = new JobModel(job);

        await _collection.InsertOneAsync(document, null, cancellationToken);
    }

    public async Task Update(Job job, CancellationToken cancellationToken = default)
    {
        var update = Builders<JobModel>.Update
            .Set(x => x.Status, job.Status)
            .Set(x => x.RetryCount, job.RetryCount);

        var result = await _collection.UpdateOneAsync(
            x => x.Id == job.Id,
            update,
            null,
            cancellationToken);
    }

    public Task<Job?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<JobModel>.Filter.Eq(x => x.Id, id);
        var project = Builders<JobModel>.Projection.Expression<Job?>(x => new Job(x.Id, x.JobType, x.Status, x.RetryCount, x.Payload, x.CreatedAt));
        var result = _collection.Find(filter)
            .Project(project)
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<IReadOnlyList<Job>> GetJobsPendingDispatch(CancellationToken cancellationToken = default)
    {
        var filter = Builders<JobModel>.Filter.Eq(x => x.Status, JobStatus.Created);
        var project = Builders<JobModel>.Projection.Expression(x => new Job(x.Id, x.JobType, x.Status, x.RetryCount, x.Payload, x.CreatedAt));

        var jobs = await _collection
            .Find(filter)
            .SortBy(x => x.CreatedAt)
            .Limit(100)
            .Project(project)
            .ToListAsync(cancellationToken);

        return jobs;
    }
}
