using SharedKernel;

namespace Domain.Organizations;

public static class OrganizationErrors
{
    public static Error NotFound(Guid organizationId) => Error.NotFound(
        "Organizations.NotFound",
        $"The organization with the Id = '{organizationId}' was not found");

    public static Error MemberNotFound(Guid userId) => Error.NotFound(
        "Organizations.MemberNotFound",
        $"The user with the Id = '{userId}' is not a member of this organization");

    public static readonly Error MemberAlreadyExists = Error.Conflict(
        "Organizations.MemberAlreadyExists",
        "The user is already a member of this organization");

    public static readonly Error CannotRemoveLastOwner = Error.Problem(
        "Organizations.CannotRemoveLastOwner",
        "Cannot remove or change the role of the last owner of the organization");

    public static readonly Error Unauthorized = Error.Failure(
        "Organizations.Unauthorized",
        "You do not have permission to perform this action on the organization");
}
