using JobQueue.Application.Abstractions;

namespace JobQueue.Application.Queries;

public class GetJobQueryHandler(IQueries queries)
{
    public Task<GetJobQueryResponse?> Handle(Guid id, CancellationToken cancellationToken = default)
        => queries.GetJob(id, cancellationToken);
}
