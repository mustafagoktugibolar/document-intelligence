using Application.Abstractions.Documents;
using DocumentFormat.OpenXml.Packaging;

namespace Infrastructure.Documents.Extractors;

internal sealed class DocxExtractor : IDocumentExtractor
{
    public bool CanHandle(string fileName)
    {
        string ext = Path.GetExtension(fileName);
        return ext.Equals(".docx", StringComparison.OrdinalIgnoreCase)
            || ext.Equals(".doc", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var doc = WordprocessingDocument.Open(stream, isEditable: false);
        string text = doc.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
        return Task.FromResult(text);
    }
}
