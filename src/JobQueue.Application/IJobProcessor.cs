using JobQueue.Domain;

namespace JobQueue.Application;

public interface IJobProcessor
{
    JobType JobType { get; }
    Task Process(string payload, CancellationToken cancellationToken = default);
}
