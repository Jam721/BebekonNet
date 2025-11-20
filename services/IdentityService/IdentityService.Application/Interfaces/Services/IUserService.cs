using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Interfaces.Services;

public interface IUserService
{
    Task Register(string userName, string email, string password, IFormFile? avatar, CancellationToken cancellationToken = default);

    Task<string> Login(string email, string password, CancellationToken cancellationToken = default);

    Task<(UserModel? user, string? token)> UpdateUser(
        Guid id,
        string? email,
        string? userName,
        string? password,
        IFormFile? avatarFile,
        CancellationToken ct = default);
    
    Task<UserModel?> GetCurrentUser(CancellationToken ct = default);
    string? GetTokenFromHeader();
}