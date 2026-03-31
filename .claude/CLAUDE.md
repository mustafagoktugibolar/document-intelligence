# Document Intelligence Platform — CLAUDE.md

## Proje Nedir?

AI destekli belge işleme platformu. Kullanıcıların yüklediği belgeleri saklayan değil, anlayan, sınıflandıran, semantik arama yapılabilir kılan ve iş kurallarıyla aksiyon üretebilen enterprise bir sistem.

## Mevcut Durum

Clean Architecture template üzerine inşa edilıyor (Users + Todos CRUD hazır). Document Intelligence feature'ları henüz başlangıç aşamasında:

| Katman | Durum |
|--------|-------|
| `Domain/Documents/DocumentStatus.cs` | Tamamlandı — enum tanımlı |
| `Domain/Documents/Document.cs` | Yapılmadı |
| `Domain/Documents/DocumentVersion.cs` | Yapılmadı |
| `Domain/Processing/ProcessingJob.cs` | Yapılmadı |
| `Domain/Events/` | Boş |
| `Domain/ValueObjects/` | Boş |
| `Application/Documents/` | Yok |
| `Infrastructure/Documents/` | Yok |
| `Web.Api/Endpoints/Documents/` | Yok |
| `Worker/` | Sadece iskelet var |

## Altyapı (Docker Compose)

- **PostgreSQL** (pgvector/pgvector:pg16) — vektör araması için pgvector extension hazır
- **RabbitMQ** 3.13 — async event pipeline için
- **Seq** — structured log arama
- **web-api** + **worker** servisleri ayrı container

## NuGet Paketleri (Directory.Packages.props'ta zaten tanımlı)

| Paket | Kullanım Amacı |
|-------|----------------|
| `Anthropic.SDK` | Claude API — summarization, Q&A, classification |
| `OpenAI` | Alternatif AI provider |
| `PdfPig` | PDF text extraction |
| `DocumentFormat.OpenXml` | DOCX/XLSX extraction |
| `Microsoft.ML.Tokenizers` | Chunking için token sayımı |
| `Pgvector.EntityFrameworkCore` | Vector similarity search |
| `RabbitMQ.Client` | Event-driven pipeline |
| `Polly` | Retry policy (AI/storage calls) |

## Domain Akış (DocumentStatus enum)

```
Uploaded → Processing → TextExtracted → Chunked → Embedded → Summarized → Completed
                                                                          ↘ Failed
```

## Planlanan Domain Modeli

### Document Aggregate
- Id, Title, OriginalFileName, MimeType, Size, Status, UploadedBy, UploadedAt, CurrentVersionId
- Domain events: DocumentUploadedDomainEvent, DocumentStatusChangedDomainEvent

### DocumentVersion Entity
- VersionNumber, FileHash, ExtractedTextReference, Language, ProcessingStatus

### ProcessingJob Aggregate (Document'tan ayrı lifecycle)
- JobId, JobType (TextExtraction / OCR / Chunking / Embedding / Summarization / Classification / Report)
- Status, RetryCount, StartedAt, CompletedAt, ErrorMessage

### DocumentChunk Entity
- ChunkIndex, Content, TokenCount, EmbeddingVector (pgvector)

### KnowledgeItem
- Chunk / Summary / Entity / Keyword abstract yapı

### PromptTemplate
- Domain objesi — AI davranışlarını sistemin parçası yapar
- contract_summary, invoice_extraction, maintenance_risk_analysis

## Planlanan Özellikler

1. **Upload & Ingestion** — PDF, DOCX, TXT, CSV, XLSX, image kabul
2. **Text Extraction** — Strategy pattern: `IDocumentExtractor` → PdfExtractor, DocxExtractor, ExcelExtractor, ImageOcrExtractor
3. **Chunking** — anlamlı parçalar, sequence no, token limiti (ML.Tokenizers)
4. **Embedding Generation** — chunk başına vektör, pgvector'da sakla
5. **Classification & Tagging** — Contract / Invoice / Maintenance Report / etc.
6. **Summarization** — executive summary, key points, risk summary
7. **Question Answering** — RAG: retrieval → chunk seç → cevap üret (kaynak göster)
8. **Similar Document Discovery** — cosine similarity ile
9. **Rule-Based Actions** — "Invoice" → finance workflow, risk skoru yüksek → alert
10. **Report Generation** — export edilebilir

## Mimari Kararlar

- **Monolith-first**, servis sınırları domain'e göre ayrışık — ileride microservice'e split edilebilir
- Worker servisi RabbitMQ'dan event tüketir, pipeline step'lerini işler
- AI provider abstraction zorunlu (`IEmbeddingService`, `ISummaryService`, vb.) — vendor lock-in yok
- Her processing step ayrı job → retry, observe edilebilir
- Versioning: aynı belgenin güncel hali yüklenebilir, önceki version korunur

## Geliştirme Sırası

### Faz 1 — Domain & Infrastructure
- [ ] Document aggregate + DocumentVersion entity
- [ ] ProcessingJob aggregate
- [ ] Domain events (DocumentUploaded, StatusChanged, JobCompleted)
- [ ] EF Core konfigürasyonları (Documents, Chunks, Jobs tabloları + pgvector)
- [ ] Migration

### Faz 2 — Upload & Ingestion
- [ ] UploadDocument command + handler
- [ ] Dosya depolama (local storage / S3 abstraction)
- [ ] DocumentUploaded event → RabbitMQ publish
- [ ] Web.Api upload endpoint

### Faz 3 — Processing Pipeline (Worker)
- [ ] RabbitMQ consumer (Worker)
- [ ] IDocumentExtractor + PdfExtractor + DocxExtractor
- [ ] Chunking service (token-aware)
- [ ] Job state machine (status transitions)

### Faz 4 — AI Layer
- [ ] IEmbeddingService + AnthropicEmbeddingService / OpenAiEmbeddingService
- [ ] ISummaryService + implementasyon
- [ ] IClassificationService + implementasyon
- [ ] Embedding'leri DB'ye yaz (pgvector)

### Faz 5 — Search & Q&A
- [ ] Semantic search endpoint (cosine similarity)
- [ ] Question answering endpoint (RAG)
- [ ] Similar document discovery

### Faz 6 — Polish
- [ ] Rule-based action engine
- [ ] Report generation
- [ ] Notification service
- [ ] Audit logging

## Kod Kuralları

- Mevcut template pattern'larını takip et (Result<T>, ICommand/IQuery, Minimal API endpoint'leri)
- Her command/query kendi klasöründe (Application/Documents/Upload/, vs.)
- Domain'de primitive obsession yok — value object kullan (DocumentId, ChunkIndex, vs.)
- AI servis interface'leri Infrastructure'da implement edilir, Application'da sadece abstraction
