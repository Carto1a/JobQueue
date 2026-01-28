namespace JobQueue.Application;

public class GetJobsQueryHandler(IQueries queries)
{
    public Task<List<GetJobQueryResponse>> Handle(CancellationToken cancellationToken = default)
        => queries.GetJobs(cancellationToken);
}
