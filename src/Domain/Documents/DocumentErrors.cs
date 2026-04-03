using SharedKernel;

namespace Domain.Documents;

public static class DocumentErrors
{
    public static Error NotFound(Guid documentId) => Error.NotFound(
        "Documents.NotFound",
        $"The document with the Id = '{documentId}' was not found");

    public static readonly Error AccessDenied = Error.Failure(
        "Documents.AccessDenied",
        "You do not have permission to access this document");

    public static readonly Error MustBeRestrictedToGrantAccess = Error.Problem(
        "Documents.MustBeRestrictedToGrantAccess",
        "Document visibility must be set to Restricted before granting individual access");

    public static readonly Error InvalidChunkRange = Error.Problem(
        "Documents.InvalidChunkRange",
        "The chunk range is invalid. StartChunkIndex must be less than or equal to EndChunkIndex");

    public static Error SectionNotFound(Guid sectionId) => Error.NotFound(
        "Documents.SectionNotFound",
        $"The section with the Id = '{sectionId}' was not found");

    public static readonly Error SectionMustBeRestrictedToGrantAccess = Error.Problem(
        "Documents.SectionMustBeRestrictedToGrantAccess",
        "Section visibility must be set to Restricted before granting individual access");
}
