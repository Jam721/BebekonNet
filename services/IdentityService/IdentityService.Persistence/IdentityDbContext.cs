using IdentitySevice.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentitySevice.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<PermissionEntity> Permissions { get; set; } = null!;
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedInitialData(modelBuilder);
    }
    
    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.Permissions)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserPermissions"));
        
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<PermissionEntity>().HasData(
            new PermissionEntity { Id = 1, Name = "Read" },
            new PermissionEntity { Id = 2, Name = "Create" },
            new PermissionEntity { Id = 3, Name = "Delete" },
            new PermissionEntity { Id = 4, Name = "Update" }
        );
    }
}