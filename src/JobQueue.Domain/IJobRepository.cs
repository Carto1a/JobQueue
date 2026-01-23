namespace JobQueue.Domain;

public interface IJobRepository
{
    Task Create(Job job, CancellationToken cancellationToken = default);
    Task Update(Job job, CancellationToken cancellationToken = default);
    Task<Job?> GetById(Guid id, CancellationToken cancellationToken = default);
}
