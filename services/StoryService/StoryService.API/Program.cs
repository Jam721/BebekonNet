using StoryService.API.Extensions;
using StoryService.Application.Interfaces.Repositories;
using StoryService.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOpenApi();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();

services.AddStoryDbContext(configuration);
services.AddScoped<IStoryRepository, StoryRepository>();

var app = builder.Build();

app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
