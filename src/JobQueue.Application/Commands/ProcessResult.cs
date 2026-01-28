namespace JobQueue.Application.Commands;

public enum ProcessResult
{
    Completed,
    Failed,
    Retry,
    NotFound
}
