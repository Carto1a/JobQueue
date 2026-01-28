using JobQueue.Domain;

namespace JobQueue.Application.Queries;

public record GetJobQueryResponse(
    Guid Id,
    JobType JobType,
    JobStatus Status,
    int RetryCount,
    DateTime CreatedAt
);
