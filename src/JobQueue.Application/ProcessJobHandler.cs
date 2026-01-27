using JobQueue.Domain;
using Microsoft.Extensions.Logging;

namespace JobQueue.Application;

public enum ProcessResult
{
    Completed,
    Failed,
    Retry,
    NotFound
}

public class ProcessJobHandler(
    IJobRepository repository,
    JobProcessorResolver resolver,
    ILogger<ProcessJobHandler> logger
)
{
    private readonly IJobRepository _repository = repository;
    private readonly JobProcessorResolver _resolver = resolver;
    private readonly ILogger<ProcessJobHandler> _logger = logger;

    public async Task<ProcessResult> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await _repository.GetById(id, cancellationToken);
        if (job is null)
        {
            _logger.LogWarning("Job {JobId} not found", id);
            return ProcessResult.NotFound;
        }

        try
        {
            job.Process();
            await _repository.Update(job, cancellationToken);

            var processor = _resolver.Resolve(job.JobType);
            await processor.Process(job.Payload, cancellationToken);

            job.Complete();
            await _repository.Update(job, cancellationToken);
            return ProcessResult.Completed;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "Job {JobId} cancelled, will retry",
                id
            );

            return ProcessResult.Retry;
        }
        catch (InvalidJobStateException exception)
        {
            _logger.LogError(exception, "Invalid state for Job {JobId}", id);
            return ProcessResult.Failed;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Job {JobId} failed", id);
            job.Fail();

            await _repository.Update(job, cancellationToken);

            if (job.CanRetry())
                return ProcessResult.Retry;

            return ProcessResult.Failed;
        }
    }
}
