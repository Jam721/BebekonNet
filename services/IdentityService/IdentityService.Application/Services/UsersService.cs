using IdentityService.Application.Interfaces.Auth;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Services;

public class UsersService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _repository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IFileStorageService _fileStorageService;

    public UsersService(
        IPasswordHasher passwordHasher, 
        IUserRepository repository, 
        IJwtProvider jwtProvider,
        IFileStorageService fileStorageService)
    {
        _passwordHasher = passwordHasher;
        _repository = repository;
        _jwtProvider = jwtProvider;
        _fileStorageService = fileStorageService;
    }
    
    public async Task Register(
        string userName, 
        string email, 
        string password, 
        IFormFile? avatar, 
        CancellationToken cancellationToken = default)
    {
        var hashedPassword = _passwordHasher.Generate(password);
    
        string avatarObjectName = "default-avatar.png";

        if (avatar != null && avatar.Length > 0)
        {
            await using var stream = avatar.OpenReadStream();
            var uploadedName = await _fileStorageService.UploadAvatarAsync(
                stream,
                avatar.FileName,
                avatar.ContentType);
            
            if (!string.IsNullOrEmpty(uploadedName))
                avatarObjectName = uploadedName;
        }

        var user = UserModel.Create(
            Guid.NewGuid(), 
            userName, 
            hashedPassword, 
            email, 
            avatarObjectName);

        await _repository.AddUserAsync(user, cancellationToken);
    }

    public async Task<string> Login(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetUserByEmail(email, cancellationToken);

        if (user == null) throw new Exception("Пользователь не найден");

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false) throw new Exception("Failed to Login");

        var token = _jwtProvider.GenerateToken(user);

        return token;
    }
    
    
}