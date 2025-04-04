using IdentityService.Application.Interfaces.Auth;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Models;

namespace IdentityService.Application.Services;

public class UsersService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _repository;
    private readonly IJwtProvider _jwtProvider;

    public UsersService(IPasswordHasher passwordHasher, IUserRepository repository, IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _repository = repository;
        _jwtProvider = jwtProvider;
    }
    
    public async Task Register(string userName, string email, string password)
    {
        var hashedPassword = _passwordHasher.Generate(password);

        var user = User.Create(Guid.NewGuid(), userName, hashedPassword, email);

        await _repository.AddUserAsync(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await _repository.GetUserByEmail(email);

        if (user == null) throw new Exception("Пользователь не найден");

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false) throw new Exception("Failed to Login");

        var token = _jwtProvider.GenerateToken(user);

        return token;
    }
    
    
}