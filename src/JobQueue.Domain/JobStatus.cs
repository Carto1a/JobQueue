namespace JobQueue.Domain;

public enum JobStatus
{
    Created,
    Pending,
    Processing,
    Concluded,
    Error,
}
