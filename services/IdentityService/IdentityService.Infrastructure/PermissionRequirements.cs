﻿using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Infrastructure;

public class PermissionRequirements : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirements(string permission)
    {
        Permission = permission;
    }
}