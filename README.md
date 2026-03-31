# Document Intelligence Platform

An AI-powered document processing platform built on Clean Architecture. Accepts documents (PDF, DOCX, TXT, CSV, XLSX, images), extracts text, chunks and embeds content, classifies documents, generates summaries, and enables semantic search and question answering via RAG.

## Architecture

```
src/
├── SharedKernel       # Result<T>, domain primitives
├── Domain             # Aggregates, entities, value objects, domain events
├── Application        # CQRS commands/queries, service abstractions
├── Infrastructure     # EF Core, pgvector, RabbitMQ, AI providers
├── Web.Api            # Minimal API endpoints
└── Worker             # Background processing pipeline (RabbitMQ consumer)
```

## Infrastructure

| Service    | Purpose                          | Default URL              |
|------------|----------------------------------|--------------------------|
| PostgreSQL | Primary DB + pgvector embeddings | `localhost:5432`         |
| RabbitMQ   | Async event pipeline             | `localhost:5672`         |
| Seq        | Structured log search            | `http://localhost:8081`  |

## Document Processing Pipeline

```
Uploaded → Processing → TextExtracted → Chunked → Embedded → Summarized → Completed
                                                                          ↘ Failed
```

Each step is a separate `ProcessingJob` — retryable and observable.

## Key Packages

| Package                        | Purpose                          |
|-------------------------------|----------------------------------|
| `Anthropic.SDK`               | Summarization, Q&A, classification |
| `PdfPig`                      | PDF text extraction              |
| `DocumentFormat.OpenXml`      | DOCX/XLSX extraction             |
| `Microsoft.ML.Tokenizers`     | Token-aware chunking             |
| `Pgvector.EntityFrameworkCore`| Vector similarity search         |
| `RabbitMQ.Client`             | Event-driven pipeline            |
| `Polly`                       | Retry policies for AI/storage    |

## Getting Started

```bash
docker compose up -d
dotnet run --project src/Web.Api
```

Build:

```bash
dotnet build DocumentIntelligence.slnx --configuration Release
```
