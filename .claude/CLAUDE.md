# Document Intelligence Platform — CLAUDE.md

## What Is This Project?

An AI-powered document processing platform. An enterprise system that doesn't just store documents — it understands them, classifies them, makes them semantically searchable, and can trigger actions based on business rules.

## Current Status

Built on a Clean Architecture template (Users + Todos CRUD are ready). Document Intelligence features are at an early stage:

| Layer | Status |
|-------|--------|
| `Domain/Documents/DocumentStatus.cs` | Done — enum defined |
| `Domain/Documents/Document.cs` | Not done |
| `Domain/Documents/DocumentVersion.cs` | Not done |
| `Domain/Processing/ProcessingJob.cs` | Not done |
| `Domain/Events/` | Empty |
| `Domain/ValueObjects/` | Empty |
| `Application/Documents/` | Missing |
| `Infrastructure/Documents/` | Missing |
| `Web.Api/Endpoints/Documents/` | Missing |
| `Worker/` | Skeleton only |

## Infrastructure (Docker Compose)

- **PostgreSQL** (pgvector/pgvector:pg16) — pgvector extension ready for vector search
- **RabbitMQ** 3.13 — for async event pipeline
- **Seq** — structured log search
- **web-api** + **worker** services in separate containers

## NuGet Packages (already defined in Directory.Packages.props)

| Package | Purpose |
|---------|---------|
| `Anthropic.SDK` | Claude API — summarization, Q&A, classification |
| `OpenAI` | Alternative AI provider |
| `PdfPig` | PDF text extraction |
| `DocumentFormat.OpenXml` | DOCX/XLSX extraction |
| `Microsoft.ML.Tokenizers` | Token counting for chunking |
| `Pgvector.EntityFrameworkCore` | Vector similarity search |
| `RabbitMQ.Client` | Event-driven pipeline |
| `Polly` | Retry policy (AI/storage calls) |

## Domain Flow (DocumentStatus enum)

```
Uploaded → Processing → TextExtracted → Chunked → Embedded → Summarized → Completed
                                                                          ↘ Failed
```

## Planned Domain Model

### Document Aggregate
- Id, Title, OriginalFileName, MimeType, Size, Status, UploadedBy, UploadedAt, CurrentVersionId
- Domain events: DocumentUploadedDomainEvent, DocumentStatusChangedDomainEvent

### DocumentVersion Entity
- VersionNumber, FileHash, ExtractedTextReference, Language, ProcessingStatus

### ProcessingJob Aggregate (separate lifecycle from Document)
- JobId, JobType (TextExtraction / OCR / Chunking / Embedding / Summarization / Classification / Report)
- Status, RetryCount, StartedAt, CompletedAt, ErrorMessage

### DocumentChunk Entity
- ChunkIndex, Content, TokenCount, EmbeddingVector (pgvector)

### KnowledgeItem
- Abstract structure: Chunk / Summary / Entity / Keyword

### PromptTemplate
- Domain object — makes AI behaviors a first-class part of the system
- contract_summary, invoice_extraction, maintenance_risk_analysis

## Planned Features

1. **Upload & Ingestion** — Accept PDF, DOCX, TXT, CSV, XLSX, image
2. **Text Extraction** — Strategy pattern: `IDocumentExtractor` → PdfExtractor, DocxExtractor, ExcelExtractor, ImageOcrExtractor
3. **Chunking** — meaningful chunks, sequence no, token limit (ML.Tokenizers)
4. **Embedding Generation** — vector per chunk, stored in pgvector
5. **Classification & Tagging** — Contract / Invoice / Maintenance Report / etc.
6. **Summarization** — executive summary, key points, risk summary
7. **Question Answering** — RAG: retrieval → select chunks → generate answer (with source citations)
8. **Similar Document Discovery** — via cosine similarity
9. **Rule-Based Actions** — "Invoice" → finance workflow, high risk score → alert
10. **Report Generation** — exportable

## Architecture Decisions

- **Monolith-first**, service boundaries separated by domain — can be split into microservices later
- Worker service consumes events from RabbitMQ, processes pipeline steps
- AI provider abstraction required (`IEmbeddingService`, `ISummaryService`, etc.) — no vendor lock-in
- Each processing step is a separate job → retryable, observable
- Versioning: a new version of the same document can be uploaded, previous versions are preserved

## Development Order

### Phase 1 — Domain & Infrastructure
- [ ] Document aggregate + DocumentVersion entity
- [ ] ProcessingJob aggregate
- [ ] Domain events (DocumentUploaded, StatusChanged, JobCompleted)
- [ ] EF Core configurations (Documents, Chunks, Jobs tables + pgvector)
- [ ] Migration

### Phase 2 — Upload & Ingestion
- [ ] UploadDocument command + handler
- [ ] File storage (local storage / S3 abstraction)
- [ ] DocumentUploaded event → RabbitMQ publish
- [ ] Web.Api upload endpoint

### Phase 3 — Processing Pipeline (Worker)
- [ ] RabbitMQ consumer (Worker)
- [ ] IDocumentExtractor + PdfExtractor + DocxExtractor
- [ ] Chunking service (token-aware)
- [ ] Job state machine (status transitions)

### Phase 4 — AI Layer
- [ ] IEmbeddingService + AnthropicEmbeddingService / OpenAiEmbeddingService
- [ ] ISummaryService + implementation
- [ ] IClassificationService + implementation
- [ ] Write embeddings to DB (pgvector)

### Phase 5 — Search & Q&A
- [ ] Semantic search endpoint (cosine similarity)
- [ ] Question answering endpoint (RAG)
- [ ] Similar document discovery

### Phase 6 — Polish
- [ ] Rule-based action engine
- [ ] Report generation
- [ ] Notification service
- [ ] Audit logging

## Code Conventions

- Follow existing template patterns (Result<T>, ICommand/IQuery, Minimal API endpoints)
- Each command/query in its own folder (Application/Documents/Upload/, etc.)
- No primitive obsession in Domain — use value objects (DocumentId, ChunkIndex, etc.)
- AI service interfaces are implemented in Infrastructure; Application layer has abstractions only
