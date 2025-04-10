using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(UserModel userModel);
}