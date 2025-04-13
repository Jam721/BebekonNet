using Microsoft.EntityFrameworkCore;
using StoryService.Persistence.Entities;

namespace StoryService.Persistence;

public class StoryDbContext(DbContextOptions<StoryDbContext> options) : DbContext(options)
{
    public DbSet<StoryEntity> Stories { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
}