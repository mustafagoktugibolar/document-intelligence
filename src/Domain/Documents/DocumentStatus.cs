namespace Domain.Documents;

public enum DocumentStatus
{
    Uploaded,
    Processing,
    TextExtracted,
    Chunked,
    Embedded,
    Summarized,
    Completed,
    Failed
}
