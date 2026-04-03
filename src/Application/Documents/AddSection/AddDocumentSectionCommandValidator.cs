using FluentValidation;

namespace Application.Documents.AddSection;

public sealed class AddDocumentSectionCommandValidator : AbstractValidator<AddDocumentSectionCommand>
{
    public AddDocumentSectionCommandValidator()
    {
        RuleFor(c => c.DocumentId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(500);
        RuleFor(c => c.StartChunkIndex).GreaterThanOrEqualTo(0);
        RuleFor(c => c.EndChunkIndex).GreaterThanOrEqualTo(c => c.StartChunkIndex);
        RuleFor(c => c.Visibility).IsInEnum();
    }
}
