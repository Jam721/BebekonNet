using System.Text;
using IdentityService.Application.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.API.Extensions;

public static class AuthExtensions
{
    public static void AddApiAuthentication(
        this IServiceCollection services, 
        JwtOptions jwtOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authorizationHeader = context.Request.Headers.Authorization.ToString();
                        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                        {
                            context.Token = authorizationHeader["Bearer ".Length..].Trim();
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSingleton<IAuthorizationHandler, PermissionsRequirementsHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(PermissionsConst.Read, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Read)))
            .AddPolicy(PermissionsConst.Create, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Create)))
            .AddPolicy(PermissionsConst.Delete, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Delete)))
            .AddPolicy(PermissionsConst.Update, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Update)));
    }
}