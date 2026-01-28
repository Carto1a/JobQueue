using JobQueue.Application;
using MongoDB.Driver;

namespace JobQueue.Infrastructure.Persistence;

public class Queries(MongoDbContext context) : IQueries
{
    private readonly IMongoCollection<JobModel> _collection = context.Jobs;

    public Task<GetJobQueryResponse?> GetJob(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<JobModel>.Filter.Eq(x => x.Id, id);
        var project = Builders<JobModel>
            .Projection
            .Expression<GetJobQueryResponse?>(x =>
                new GetJobQueryResponse(x.Id, x.JobType, x.Status, x.RetryCount, x.CreatedAt));

        var result = _collection
            .Find(filter)
            .Project(project)
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public Task<List<GetJobQueryResponse>> GetJobs(CancellationToken cancellationToken = default)
    {
        var project = Builders<JobModel>
            .Projection
            .Expression(x => new GetJobQueryResponse(x.Id, x.JobType, x.Status, x.RetryCount, x.CreatedAt));

        var result = _collection
            .Find(Builders<JobModel>.Filter.Empty)
            .Project(project)
            .ToListAsync(cancellationToken);

        return result;
    }
}
