namespace Application.Abstractions.AI;

public interface ISummaryService
{
    Task<string> SummarizeAsync(string text, CancellationToken cancellationToken);
}
