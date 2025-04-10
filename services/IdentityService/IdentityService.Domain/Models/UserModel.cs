namespace IdentityService.Domain.Models;

public class UserModel
{
    private UserModel(Guid id, string userName, string passwordHash, string email)
    {
        Id = id;
        UserName = userName;
        PasswordHash = passwordHash;
        Email = email;
    }
    public UserModel() {}
    
    public Guid Id { get; set; }

    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    
    public ICollection<PermissionModel> Permisions { get; set; } 

    public static UserModel Create(Guid id, string userName, string passwordHash, string email)
    {
        return new UserModel(id, userName, passwordHash, email);
    }
}