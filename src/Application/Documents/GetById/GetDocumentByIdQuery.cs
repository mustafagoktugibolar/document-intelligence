using Application.Abstractions.Messaging;

namespace Application.Documents.GetById;

public sealed record GetDocumentByIdQuery(Guid DocumentId) : IQuery<DocumentResponse>;
