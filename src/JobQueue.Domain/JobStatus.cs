namespace JobQueue.Domain;

public enum JobStatus
{
    Pending,
    Processing,
    Concluded,
    Error,
}
