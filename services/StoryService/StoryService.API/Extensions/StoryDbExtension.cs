using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StoryService.Persistence;

namespace StoryService.API.Extensions;

public static class StoryDbExtension
{
    public static void AddStoryDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoryDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));

            options.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
    }
}