using JobQueue.Domain;
using Microsoft.Extensions.Logging;

namespace JobQueue.Application;

public class DispatchPendingJobsHandler(
    ILogger<DispatchPendingJobsHandler> logger,
    IJobRepository repository,
    IQueuePublisher publisher
)
{
    private readonly ILogger<DispatchPendingJobsHandler> _logger = logger;
    private readonly IJobRepository _repository = repository;
    private readonly IQueuePublisher _publisher = publisher;

    public async Task Handle(CancellationToken cancellationToken = default)
    {
        var jobs = await _repository.GetJobsPendingDispatch(cancellationToken);

        foreach (var job in jobs)
        {
            try
            {
                await _publisher.PublishJob(job.Id);
                job.MarkAsPending();
                await _repository.Update(job);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to dispatch job {JobId}", job.Id);
            }
        }
    }
}
