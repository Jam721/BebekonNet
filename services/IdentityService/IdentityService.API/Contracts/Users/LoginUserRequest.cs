using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.Contracts.Users;

public class LoginUserRequest
{
    [Required] public string Email { get; set; }
    
    [Required] public string Password { get; set; }
}