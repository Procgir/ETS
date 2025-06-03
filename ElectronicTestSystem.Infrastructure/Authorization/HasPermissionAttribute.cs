using Microsoft.AspNetCore.Authorization;

namespace ElectronicTestSystem.Infrastructure.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(permission)
    {
    }
}
