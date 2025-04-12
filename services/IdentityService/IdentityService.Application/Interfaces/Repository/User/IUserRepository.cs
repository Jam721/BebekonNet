
using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces.Repository.User;

public interface IUserRepository
{
    Task AddUserAsync(UserModel userModel, CancellationToken ct = default);
    Task<UserModel?> GetUserByEmail(string email, CancellationToken ct = default);
    Task<ICollection<PermissionModel>> GetPermissionByEmail(string email, CancellationToken ct = default);
}