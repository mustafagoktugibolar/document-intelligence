namespace Application.Abstractions.Documents;

public interface IDocumentExtractor
{
    bool CanHandle(string fileName);
    Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken);
}
