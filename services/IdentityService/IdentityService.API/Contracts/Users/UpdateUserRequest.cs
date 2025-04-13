using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.Contracts.Users;

public class UpdateUserRequest
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    
    [DataType(DataType.Upload)]
    public IFormFile? AvatarFile { get; set; }
}