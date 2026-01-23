using JobQueue.Domain;

namespace JobQueue.Application;

public class JobProcessorResolver(IEnumerable<IJobProcessor> processors)
{
    private readonly IEnumerable<IJobProcessor> _processors = processors;

    public IJobProcessor Resolve(JobType jobType)
    {
        var processor = _processors.FirstOrDefault(x => x.JobType == jobType);

        if (processor is null)
            throw new Exception($"No porcessor found for job type {jobType}");

        return processor;
    }
}
