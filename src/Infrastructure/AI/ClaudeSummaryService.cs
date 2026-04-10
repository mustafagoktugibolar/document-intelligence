using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Application.Abstractions.AI;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.AI;

internal sealed class ClaudeSummaryService : ISummaryService, IDisposable
{
    private readonly AnthropicClient _client;

    public ClaudeSummaryService(IConfiguration configuration)
    {
        string apiKey = configuration["AI:Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("AI:Anthropic:ApiKey configuration is missing");

        _client = new AnthropicClient(new APIAuthentication(apiKey));
    }

    public async Task<string> SummarizeAsync(string text, CancellationToken cancellationToken)
    {
        string prompt = $"""
            Summarize the following document in 3-5 concise sentences.
            Focus on the main purpose, key points, and any important conclusions.
            Respond with the summary only, no preamble.

            Document:
            {text[..Math.Min(text.Length, 8000)]}
            """;

        MessageResponse response = await _client.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Messages =
                [
                    new Message
                    {
                        Role = RoleType.User,
                        Content = [new TextContent { Text = prompt }]
                    }
                ],
                MaxTokens = 512,
                Model = "claude-3-5-haiku-20241022"
            },
            cancellationToken);

        return response.Content.OfType<TextContent>().First().Text;
    }

    public void Dispose() => _client.Dispose();
}
