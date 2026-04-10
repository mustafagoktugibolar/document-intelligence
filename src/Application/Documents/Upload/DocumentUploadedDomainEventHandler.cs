using Application.Abstractions.Messaging;
using Domain.Documents;
using SharedKernel;

namespace Application.Documents.Upload;

internal sealed class DocumentUploadedDomainEventHandler(IRabbitMQPublisher publisher)
    : IDomainEventHandler<DocumentUploadedDomainEvent>
{
    public Task Handle(DocumentUploadedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var message = new DocumentUploadedMessage(
            domainEvent.DocumentId,
            domainEvent.OrganizationId,
            domainEvent.UploadedByUserId);

        return publisher.PublishAsync(message, "document.uploaded", cancellationToken);
    }
}
