using System.Text;
using Application.Abstractions.Documents;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Infrastructure.Documents.Extractors;

internal sealed class PdfExtractor : IDocumentExtractor
{
    public bool CanHandle(string fileName) =>
        Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);

    public Task<string> ExtractTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var pdf = PdfDocument.Open(stream);

        StringBuilder sb = new();
        foreach (Page page in pdf.GetPages())
        {
            sb.AppendLine(page.Text);
        }

        return Task.FromResult(sb.ToString());
    }
}
