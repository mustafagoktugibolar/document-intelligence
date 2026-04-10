using System.ClientModel;
using Application.Abstractions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;

namespace Infrastructure.AI;

internal sealed class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _client;

    public OpenAiEmbeddingService(IConfiguration configuration)
    {
        string apiKey = configuration["AI:OpenAi:ApiKey"]
            ?? throw new InvalidOperationException("AI:OpenAi:ApiKey configuration is missing");

        _client = new EmbeddingClient("text-embedding-3-small", new ApiKeyCredential(apiKey));
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        ClientResult<OpenAIEmbedding> result = await _client.GenerateEmbeddingAsync(
            text, cancellationToken: cancellationToken);

        return result.Value.ToFloats().ToArray();
    }
}
