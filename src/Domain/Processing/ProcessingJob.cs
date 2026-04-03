using SharedKernel;

namespace Domain.Processing;

public enum JobType
{
    TextExtraction,
    OCR,
    Chunking,
    Embedding,
    Summarization,
    Classification,
    Report
}

public enum JobStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public sealed class ProcessingJob : Entity
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public JobType JobType { get; set; }
    public JobStatus JobStatus { get; set; }
    public int RetryCount { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public static ProcessingJob Create(Guid documentId, JobType jobType) =>
        new()
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            JobType = jobType,
            JobStatus = JobStatus.Pending,
            RetryCount = 0
        };

    public void Start()
    {
        JobStatus = JobStatus.Running;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        JobStatus = JobStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void Fail(string errorMessage)
    {
        JobStatus = JobStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public bool CanRetry(int maxRetries) =>
        JobStatus == JobStatus.Failed && RetryCount < maxRetries;
}
