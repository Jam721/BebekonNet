using System.Text;
using IdentityService.Infrastructure;
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
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["tasty"];

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSingleton<IAuthorizationHandler, PermissionsRequirementsHandler>();

        services.AddAuthorization(x=>
        {
            x.AddPolicy(PermissionsConst.Read, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Read)));
            x.AddPolicy(PermissionsConst.Create, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Create)));
            x.AddPolicy(PermissionsConst.Delete, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Delete)));
            x.AddPolicy(PermissionsConst.Update, builder => builder
                .Requirements.Add(new PermissionRequirements(PermissionsConst.Update)));
        });
    }
}