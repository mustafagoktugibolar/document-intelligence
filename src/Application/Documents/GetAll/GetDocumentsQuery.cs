using Application.Abstractions.Messaging;

namespace Application.Documents.GetAll;

public sealed record GetDocumentsQuery(Guid OrganizationId) : IQuery<List<DocumentSummaryResponse>>;
