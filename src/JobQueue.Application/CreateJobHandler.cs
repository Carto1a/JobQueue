using JobQueue.Domain;
using Microsoft.Extensions.Logging;

namespace JobQueue.Application;

public record CreateJobCommand(JobType JobType, string Payload);

public class CreateJobHandler(
    IJobRepository repository,
    IQueuePublisher publisher,
    ILogger<CreateJobHandler> logger
)
{
    private readonly IJobRepository _repository = repository;
    private readonly IQueuePublisher _publisher = publisher;
    private readonly ILogger<CreateJobHandler> _logger = logger;

    public async Task<Guid> Handle(CreateJobCommand request, CancellationToken cancellationToken = default)
    {
        var job = Job.Create(request.JobType, request.Payload);

        await _repository.Create(job, cancellationToken);

        // NOTE: seria melhor usar domain event para não deixar acoplado
        try
        {
            await _publisher.PublishJob(job.Id);
            job.MarkAsPending();
            await _repository.Update(job, CancellationToken.None);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to publish job {JobId}", job.Id);
        }

        return job.Id;
    }
}
