namespace Application.Documents;

public sealed record DocumentUploadedMessage(
    Guid DocumentId,
    Guid OrganizationId,
    Guid UploadedByUserId);
