using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Interfaces.Services;

public interface IUserService
{
    Task Register(string userName, string email, string password, IFormFile? avatar, CancellationToken cancellationToken = default);

    Task<string> Login(string email, string password, CancellationToken cancellationToken = default);
}