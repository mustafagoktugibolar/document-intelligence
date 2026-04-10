namespace Application.Abstractions.AI;

public interface IClassificationService
{
    Task<string> ClassifyAsync(string text, string fileName, CancellationToken cancellationToken);
}
