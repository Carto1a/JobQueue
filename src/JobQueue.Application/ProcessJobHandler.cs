using JobQueue.Domain;

namespace JobQueue.Application;

public class ProcessJobHandler(IJobRepository repository, JobProcessorResolver resolver)
{
    private readonly IJobRepository _repository = repository;
    private readonly JobProcessorResolver _resolver = resolver;

    public async Task Handle(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await _repository.GetById(id, cancellationToken);
        if (job is null)
            throw new Exception();

        try
        {
            job.Process();
            await _repository.Update(job, cancellationToken);

            var processor = _resolver.Resolve(job.JobType);
            await processor.Process(job.Payload, cancellationToken);

            job.Complete();
        }
        catch
        {
            job.Fail();
        }

        await _repository.Update(job, cancellationToken);
    }
}
