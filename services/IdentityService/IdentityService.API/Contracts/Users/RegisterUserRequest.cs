using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.Contracts.Users;

public class RegisterUserRequest
{
    [Required] public string UserName { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    
    [DataType(DataType.Upload)]
    public IFormFile AvatarFile { get; set; }
}