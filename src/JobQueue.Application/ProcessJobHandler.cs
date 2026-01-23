using JobQueue.Domain;
using Microsoft.Extensions.Logging;

namespace JobQueue.Application;

public class ProcessJobHandler(
    IJobRepository repository,
    JobProcessorResolver resolver,
    ILogger<ProcessJobHandler> logger
)
{
    private readonly IJobRepository _repository = repository;
    private readonly JobProcessorResolver _resolver = resolver;
    private readonly ILogger<ProcessJobHandler> _logger = logger;

    public async Task Handle(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await _repository.GetById(id, cancellationToken);
        if (job is null)
        {
            _logger.LogWarning("Job {JobId} not found", id);
            return;
        }

        try
        {
            job.Process();
            await _repository.Update(job, cancellationToken);

            var processor = _resolver.Resolve(job.JobType);
            await processor.Process(job.Payload, cancellationToken);

            job.Complete();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Job {JobId} failed", id);
            job.Fail();
        }

        await _repository.Update(job, cancellationToken);
    }
}
