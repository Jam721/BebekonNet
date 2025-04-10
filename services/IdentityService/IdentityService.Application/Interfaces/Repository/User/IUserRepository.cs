
using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces.Repository.User;

public interface IUserRepository
{
    Task AddUserAsync(UserModel userModel);
    Task<UserModel?> GetUserByEmail(string email);
    Task<ICollection<PermissionModel>> GetPermissionByEmail(string email);
}