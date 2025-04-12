using IdentityService.API.Extensions;
using IdentityService.API.Validation;
using IdentityService.Application.Interfaces.Auth;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Application.Services;
using IdentityService.Infrastructure;
using IdentitySevice.Persistence;
using IdentitySevice.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Minio;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddApiAuthentication(configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();

services.AddDbContextExtensions(configuration);

builder.Services.AddMinio(configureClient => 
{
    var endpoint = builder.Configuration["Minio:Endpoint"];
    var accessKey = builder.Configuration["Minio:AccessKey"];
    var secretKey = builder.Configuration["Minio:SecretKey"];
    var useSsl = bool.Parse(builder.Configuration["Minio:UseSSL"] ?? "false");

    configureClient
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL(useSsl);
});
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IUserRepository, UserRepository>();

services.AddScoped<IUserService, UsersService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    context.Database.EnsureCreated();
}

app.UseMiddleware<FileValidationMiddleware>();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();