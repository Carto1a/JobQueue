namespace JobQueue.Domain;

class Job
{
    public Guid Id { get; private set; }
    public JobType JobType { get; private set; }
    public JobStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public string Payload { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Job(Guid id, JobType jobType, JobStatus status, int retryCount, string payload, DateTime createdAt)
    {
        Id = id;
        JobType = jobType;
        Status = status;
        RetryCount = retryCount;
        Payload = payload;
        CreatedAt = createdAt;
    }

    public static Job Create(JobType jobType, string payload)
    {
        return new Job(Guid.NewGuid(), jobType, JobStatus.Pending, 0, payload, DateTime.UtcNow);
    }

    public void Process()
    {
        EnsureStatus(JobStatus.Pending);
        Status = JobStatus.Processing;
    }

    public void Complete()
    {
        EnsureStatus(JobStatus.Processing);
        Status = JobStatus.Concluded;
    }

    public void Fail()
    {
        EnsureStatus(JobStatus.Processing);
        RetryCount++;

        if (RetryCount >= GetMaxAttempts())
        {
            Status = JobStatus.Error;
            return;
        }

        Status = JobStatus.Pending;
    }

    private void EnsureStatus(JobStatus expected)
    {
        if (Status != expected)
            throw new Exception($"Invalid state. Expected {expected}, current {Status}");
    }

    private int GetMaxAttempts()
    {
        return JobType switch
        {
            JobType.SendEmail => 5,
            JobType.GenerateReport => 3,
            _ => 3
        };
    }
}
