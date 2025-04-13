
using IdentityService.Domain.Models;

namespace IdentityService.Application.Interfaces.Repository.User;

public interface IUserRepository
{
    Task<List<UserModel>> GetAllUsersAsync(CancellationToken ct = default);
    Task AddUserAsync(UserModel userModel, CancellationToken ct = default);
    Task<UserModel?> UpdateUserByEmail(
        Guid id,
        string email, string userName, 
        string password, string avatarUrl,
        CancellationToken ct = default);
    Task<UserModel?> GetUserByEmail(string email, CancellationToken ct = default);
    Task<UserModel?> GetUserById(Guid id, CancellationToken ct = default);
    Task<ICollection<PermissionModel>> GetPermissionByEmail(string email, CancellationToken ct = default);
    Task GiveAuthorshipAsync(string email, CancellationToken ct = default);
}