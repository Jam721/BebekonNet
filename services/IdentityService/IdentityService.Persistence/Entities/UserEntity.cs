namespace IdentitySevice.Persistence.Entities;

public class UserEntity
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PermissionEntity> Permissions { get; set; } = new();
}