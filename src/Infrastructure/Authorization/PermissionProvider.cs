namespace Infrastructure.Authorization;

internal sealed class PermissionProvider
{
    public Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        HashSet<string> permissionsSet =
        [
            "users:access",
            "organizations:access",
            "organizations:manage",
            "documents:read",
            "documents:write",
            "documents:manage"
        ];

        return Task.FromResult(permissionsSet);
    }
}
