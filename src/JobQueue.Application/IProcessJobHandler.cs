namespace JobQueue.Application;

public interface IProcessJobHandler
{
    Task<ProcessResult> Handle(Guid id, CancellationToken cancellationToken);
}
