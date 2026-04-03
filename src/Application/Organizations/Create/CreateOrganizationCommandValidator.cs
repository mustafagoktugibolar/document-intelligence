using FluentValidation;

namespace Application.Organizations.Create;

public sealed class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
    }
}
