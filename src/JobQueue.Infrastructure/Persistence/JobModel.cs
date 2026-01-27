using JobQueue.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JobQueue.Infrastructure.Persistence;

public class JobModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public JobType JobType { get; private set; }
    public JobStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public string Payload { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public JobModel(Job job)
    {
        Id = job.Id;
        JobType = job.JobType;
        Status = job.Status;
        RetryCount = job.RetryCount;
        Payload = job.Payload;
        CreatedAt = job.CreatedAt;
    }
}
