using System.Text;
using System.Text.Json;
using Application.Abstractions.AI;
using Application.Abstractions.Data;
using Application.Abstractions.Documents;
using Application.Abstractions.Storage;
using Application.Documents;
using Domain.Documents;
using Domain.Processing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker;

public class DocumentWorker(
    ILogger<DocumentWorker> logger,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration)
    : BackgroundService
{
    private const string ExchangeName = "document-intelligence";
    private const string QueueName = "document.uploaded";
    private const string RoutingKey = "document.uploaded";

    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMQ:Port"], out int port) ? port : 5672,
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: cancellationToken);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Connected to RabbitMQ. Listening on queue: {Queue}", QueueName);
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new(_channel!);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel!.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
    {
        string json = Encoding.UTF8.GetString(args.Body.ToArray());

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received message on routing key: {RoutingKey}", args.RoutingKey);
        }

        try
        {
            DocumentUploadedMessage? message = JsonSerializer.Deserialize<DocumentUploadedMessage>(json);
            if (message is null)
            {
                logger.LogWarning("Could not deserialize message body, discarding");
                await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            await ProcessDocumentUploadedAsync(message);
            await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message for document");
            await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private async Task ProcessDocumentUploadedAsync(DocumentUploadedMessage message)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IApplicationDbContext context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        IFileStorageService storageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
        IEnumerable<IDocumentExtractor> extractors = scope.ServiceProvider.GetRequiredService<IEnumerable<IDocumentExtractor>>();
        IChunkingService chunkingService = scope.ServiceProvider.GetRequiredService<IChunkingService>();
        IEmbeddingService embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
        ISummaryService summaryService = scope.ServiceProvider.GetRequiredService<ISummaryService>();
        IClassificationService classificationService = scope.ServiceProvider.GetRequiredService<IClassificationService>();

        Document? document = await context.Documents
            .FirstOrDefaultAsync(d => d.Id == message.DocumentId);

        if (document is null)
        {
            logger.LogWarning("Document {DocumentId} not found, skipping", message.DocumentId);
            return;
        }

        document.UpdateStatus(DocumentStatus.Processing);

        var job = ProcessingJob.Create(message.DocumentId, JobType.TextExtraction);
        job.Start();
        context.ProcessingJobs.Add(job);
        await context.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Started {JobType} job {JobId} for document {DocumentId}",
                job.JobType, job.Id, message.DocumentId);
        }

        try
        {
            // --- Extract text ---
            IDocumentExtractor? extractor = extractors.FirstOrDefault(e => e.CanHandle(document.FileName));
            if (extractor is null)
            {
                await FailJobAsync(job, context, $"No extractor found for '{document.FileName}'");
                return;
            }

            await using Stream fileStream = await storageService.DownloadAsync(document.StorageKey, CancellationToken.None);
            string extractedText = await extractor.ExtractTextAsync(fileStream, CancellationToken.None);
            document.UpdateStatus(DocumentStatus.TextExtracted);

            // --- Chunk ---
            IReadOnlyList<DocumentChunk> chunks = chunkingService.Chunk(extractedText, document.Id);
            foreach (DocumentChunk chunk in chunks)
            {
                context.DocumentChunks.Add(chunk);
            }

            document.SetTotalChunks(chunks.Count);
            document.UpdateStatus(DocumentStatus.Chunked);
            await context.SaveChangesAsync();

            // --- Embed ---
            foreach (DocumentChunk chunk in chunks)
            {
                chunk.EmbeddingVector = await embeddingService.GenerateEmbeddingAsync(
                    chunk.Content, CancellationToken.None);
            }

            document.UpdateStatus(DocumentStatus.Embedded);
            await context.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Embedded {ChunkCount} chunks for document {DocumentId}",
                    chunks.Count, message.DocumentId);
            }

            // --- Summarize & classify ---
            string summary = await summaryService.SummarizeAsync(extractedText, CancellationToken.None);
            string classification = await classificationService.ClassifyAsync(
                extractedText, document.FileName, CancellationToken.None);

            document.SetSummary(summary);
            document.SetClassification(classification);
            document.UpdateStatus(DocumentStatus.Summarized);
            job.Complete();
            await context.SaveChangesAsync();

            document.UpdateStatus(DocumentStatus.Completed);
            await context.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Completed job {JobId} for document {DocumentId}: classification={Classification}",
                    job.Id, message.DocumentId, classification);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for document {DocumentId}", message.DocumentId);
            await FailJobAsync(job, context, ex.Message);
        }
    }

    private static async Task FailJobAsync(ProcessingJob job, IApplicationDbContext context, string errorMessage)
    {
        job.Fail(errorMessage);
        await context.SaveChangesAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
