using JobQueue.Application.Abstractions;
using JobQueue.Domain;

namespace JobQueue.Infrastructure.Processors;

public class EmailJobProcessor : IJobProcessor
{
    public JobType JobType => JobType.SendEmail;

    public async Task Process(string payload, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        await Task.Delay(random.Next(100, 1200));

        if (random.NextDouble() < 0.3)
        {
            throw new Exception("Simulated random failure during job processing.");
        }
    }
}
