using System.Text.Json.Serialization;

namespace IdentityService.Domain.Models;

public class UserModel
{
    private UserModel(Guid id, string userName, string passwordHash, string email, string avatarUrl)
    {
        Id = id;
        UserName = userName;
        PasswordHash = passwordHash;
        Email = email;
        AvatarUrl = avatarUrl;
    }
    public UserModel() {}
    
    public Guid Id { get; set; }

    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    
    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PermissionModel> Permisions { get; set; } = new();

    public static UserModel Create(Guid id, string userName, string passwordHash, string email, string? avatarUrl)
    {
        return new UserModel(id, userName, passwordHash, email, avatarUrl);
    }
}