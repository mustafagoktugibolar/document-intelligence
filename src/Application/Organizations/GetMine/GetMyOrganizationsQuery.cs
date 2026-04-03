using Application.Abstractions.Messaging;

namespace Application.Organizations.GetMine;

public sealed record GetMyOrganizationsQuery : IQuery<List<OrganizationSummaryResponse>>;
