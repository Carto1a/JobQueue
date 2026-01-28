using JobQueue.Application;
using JobQueue.Domain;

namespace JobQueue.Infrastructure.Processors;

public class ReportJobProcessor : IJobProcessor
{
    public JobType JobType => JobType.GenerateReport;

    public async Task Process(string payload, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        await Task.Delay(random.Next(100, 10000));

        if (random.NextDouble() < 0.3)
        {
            throw new Exception("Simulated random failure during job processing.");
        }
    }
}
