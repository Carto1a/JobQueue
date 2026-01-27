namespace JobQueue.Application;

public enum ProcessResult
{
    Completed,
    Failed,
    Retry,
    NotFound
}
