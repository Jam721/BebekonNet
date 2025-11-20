using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Application.Services.Auth;

public class PermissionRequirements(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}