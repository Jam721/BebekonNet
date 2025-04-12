using IdentityService.API.Extensions;
using IdentityService.Application.Interfaces.Auth;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Application.Services;
using IdentityService.Infrastructure;
using IdentitySevice.Persistence;
using IdentitySevice.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddApiAuthentication(configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();

services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("Database"));
    
    options.ConfigureWarnings(w => 
        w.Ignore(RelationalEventId.PendingModelChangesWarning));
});

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