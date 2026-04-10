using Application.Abstractions.Documents;

namespace Infrastructure.Documents.Extractors;

internal sealed class TxtExtractor : IDocumentExtractor
{
    public bool CanHandle(string fileName)
    {
        string ext = Path.GetExtension(fileName);
        return ext.Equals(".txt", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".csv", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".md", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        using StreamReader reader = new(stream, leaveOpen: true);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
