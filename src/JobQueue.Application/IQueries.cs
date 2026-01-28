namespace JobQueue.Application;

public interface IQueries
{
    Task<List<GetJobQueryResponse>> GetJobs(CancellationToken cancellationToken = default);
    Task<GetJobQueryResponse?> GetJob(Guid id, CancellationToken cancellationToken = default);
}
