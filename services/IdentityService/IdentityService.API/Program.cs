using IdentityService.API.Extensions;
using IdentityService.API.Validation;
using IdentityService.Application.Interfaces.Auth;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Application.Services;
using IdentityService.Application.Services.Auth;
using IdentitySevice.Persistence;
using IdentitySevice.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddApiAuthentication(configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();

services.AddDbContextExtensions(configuration);

services.AddCors(options => {
    options.AddPolicy("ReactPolicy", policy => {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

services.AddMinioExtension(configuration);

services.AddScoped<IFileStorageService, FileStorageService>();

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

app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();