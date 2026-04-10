using System.Text;
using Application.Abstractions.Documents;
using Domain.Documents;
using Microsoft.ML.Tokenizers;

namespace Infrastructure.Documents;

internal sealed class ChunkingService : IChunkingService
{
    private const int DefaultMaxTokens = 512;
    private readonly Tokenizer _tokenizer;

    public ChunkingService()
    {
        _tokenizer = TiktokenTokenizer.CreateForEncoding("cl100k_base");
    }

    public IReadOnlyList<DocumentChunk> Chunk(string text, Guid documentId, int maxTokensPerChunk = DefaultMaxTokens)
    {
        string[] paragraphs = text.Split(
            ["\r\n\r\n", "\n\n"],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<DocumentChunk> chunks = [];
        StringBuilder currentContent = new();
        int currentTokens = 0;
        int chunkIndex = 0;

        foreach (string paragraph in paragraphs)
        {
            if (string.IsNullOrWhiteSpace(paragraph))
            {
                continue;
            }

            int paragraphTokens = _tokenizer.CountTokens(paragraph);

            if (currentTokens + paragraphTokens > maxTokensPerChunk && currentContent.Length > 0)
            {
                chunks.Add(CreateChunk(documentId, chunkIndex++, currentContent.ToString().Trim(), currentTokens));
                currentContent.Clear();
                currentTokens = 0;
            }

            if (paragraphTokens > maxTokensPerChunk)
            {
                if (currentContent.Length > 0)
                {
                    chunks.Add(CreateChunk(documentId, chunkIndex++, currentContent.ToString().Trim(), currentTokens));
                    currentContent.Clear();
                    currentTokens = 0;
                }

                foreach ((string Content, int Tokens) split in SplitLargeParagraph(paragraph, maxTokensPerChunk))
                {
                    chunks.Add(CreateChunk(documentId, chunkIndex++, split.Content, split.Tokens));
                }

                continue;
            }

            if (currentContent.Length > 0)
            {
                currentContent.AppendLine();
            }

            currentContent.Append(paragraph);
            currentTokens += paragraphTokens;
        }

        if (currentContent.Length > 0)
        {
            chunks.Add(CreateChunk(documentId, chunkIndex, currentContent.ToString().Trim(), currentTokens));
        }

        return chunks;
    }

    private IEnumerable<(string Content, int Tokens)> SplitLargeParagraph(string text, int maxTokens)
    {
        string[] sentences = text.Split([". ", ".\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        StringBuilder currentContent = new();
        int currentTokens = 0;

        foreach (string sentence in sentences)
        {
            int sentenceTokens = _tokenizer.CountTokens(sentence);

            if (currentTokens + sentenceTokens > maxTokens && currentContent.Length > 0)
            {
                yield return (currentContent.ToString().Trim(), currentTokens);
                currentContent.Clear();
                currentTokens = 0;
            }

            if (currentContent.Length > 0)
            {
                currentContent.Append(". ");
            }

            currentContent.Append(sentence);
            currentTokens += sentenceTokens;
        }

        if (currentContent.Length > 0)
        {
            yield return (currentContent.ToString().Trim(), currentTokens);
        }
    }

    private static DocumentChunk CreateChunk(Guid documentId, int index, string content, int tokenCount) =>
        new()
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            ChunkIndex = index,
            Content = content,
            TokenCount = tokenCount
        };
}
