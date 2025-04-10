using System.Security.Claims;
using IdentityService.Application.Interfaces.Repository.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public class PermissionsRequirementsHandler(IServiceScopeFactory scopeFactory) : AuthorizationHandler<PermissionRequirements>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirements requirement)
    {
        var userEmail = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail)) return;

        using var scope = scopeFactory.CreateScope();
        var accountRepo = scope.ServiceProvider.GetService<IUserRepository>();
    
        if (accountRepo == null)
        {
            return;
        }

        try
        {
            var permissions = await accountRepo.GetPermissionByEmail(userEmail);

            if (permissions?.Any(x => x.Name == requirement.Permission) == true)
            {
                context.Succeed(requirement);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}