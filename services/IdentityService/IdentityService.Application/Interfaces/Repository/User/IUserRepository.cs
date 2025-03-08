
namespace IdentityService.Application.Interfaces.Repository.User;

public interface IUserRepository
{
    Task AddUserAsync(Domain.Models.User user);
    Task<Domain.Models.User?> GetUserByEmail(string email);
}