namespace JobQueue.Domain;

public class DomainException(string message) : Exception(message);

public sealed class InvalidJobStateException(
    JobStatus expected,
    JobStatus current
) : DomainException($"Invalid job state. Expected {expected}, current {current}.")
{
    public JobStatus Expected { get; } = expected;
    public JobStatus Current { get; } = current;
}
