using FluentValidation;

namespace Application.Documents.Upload;

public sealed class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(c => c.OrganizationId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty().MaximumLength(500);
        RuleFor(c => c.StorageKey).NotEmpty().MaximumLength(1000);
        RuleFor(c => c.FileSizeBytes).GreaterThan(0);
    }
}
