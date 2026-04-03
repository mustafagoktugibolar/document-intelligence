using SharedKernel;

namespace Domain.Documents;

public sealed record DocumentUploadedDomainEvent(
    Guid DocumentId,
    Guid OrganizationId,
    Guid UploadedByUserId) : IDomainEvent;
