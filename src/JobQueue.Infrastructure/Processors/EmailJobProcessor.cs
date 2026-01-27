using JobQueue.Application;
using JobQueue.Domain;

namespace JobQueue.Infrastructure.Processors;

public class EmailJobProcessor : IJobProcessor
{
    public JobType JobType => JobType.SendEmail;

    public Task Process(string payload, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        return Task.Delay(random.Next(100, 500));
    }
}
