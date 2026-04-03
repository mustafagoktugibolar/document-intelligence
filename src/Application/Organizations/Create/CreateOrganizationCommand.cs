using Application.Abstractions.Messaging;

namespace Application.Organizations.Create;

public sealed class CreateOrganizationCommand : ICommand<Guid>
{
    public string Name { get; set; } = string.Empty;
}
