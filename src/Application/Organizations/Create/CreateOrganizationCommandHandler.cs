using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Organizations;
using SharedKernel;

namespace Application.Organizations.Create;

internal sealed class CreateOrganizationCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : ICommandHandler<CreateOrganizationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = Organization.Create(command.Name, userContext.UserId);

        context.Organizations.Add(organization);

        await context.SaveChangesAsync(cancellationToken);

        return organization.Id;
    }
}
