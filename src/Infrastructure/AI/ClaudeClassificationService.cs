using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Application.Abstractions.AI;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.AI;

internal sealed class ClaudeClassificationService : IClassificationService, IDisposable
{
    private readonly AnthropicClient _client;

    public ClaudeClassificationService(IConfiguration configuration)
    {
        string apiKey = configuration["AI:Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("AI:Anthropic:ApiKey configuration is missing");

        _client = new AnthropicClient(new APIAuthentication(apiKey));
    }

    public async Task<string> ClassifyAsync(string text, string fileName, CancellationToken cancellationToken)
    {
        string prompt = $"""
            Classify the following document into exactly one of these categories:
            Invoice, Contract, Report, Maintenance, Legal, Financial, Technical, HR, Other

            File name: {fileName}
            Document excerpt:
            {text[..Math.Min(text.Length, 4000)]}

            Respond with the category name only, nothing else.
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
                MaxTokens = 32,
                Model = "claude-3-5-haiku-20241022"
            },
            cancellationToken);

        return response.Content.OfType<TextContent>().First().Text.Trim();
    }

    public void Dispose() => _client.Dispose();
}
