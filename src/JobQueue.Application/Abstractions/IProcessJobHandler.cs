using JobQueue.Application.Commands;

namespace JobQueue.Application.Abstractions;

public interface IProcessJobHandler
{
    Task<ProcessResult> Handle(Guid id, CancellationToken cancellationToken);
}
