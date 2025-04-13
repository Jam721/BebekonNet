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

    public async Task<(UserModel? user, string? token)> UpdateUser(
        Guid id,
        string? email,
        string? userName,
        string? password,
        IFormFile? avatarFile,
        CancellationToken ct = default)
    {
        var user = await _repository.GetUserById(id, ct);
        if (user == null)
            throw new ArgumentException("User not found");
        
        string? hashedPassword = null;
        if (!string.IsNullOrEmpty(password))
        {
            hashedPassword = _passwordHasher.Generate(password);
        }
        
        string? avatarUrl = user.AvatarUrl;
        if (avatarFile != null && avatarFile.Length > 0)
        {
            if (avatarUrl != "default-avatar.png")
            {
                await _fileStorageService.DeleteAvatarAsync(avatarUrl);
            }
            
            await using var stream = avatarFile.OpenReadStream();
            var uploadedName = await _fileStorageService.UploadAvatarAsync(
                stream,
                avatarFile.FileName,
                avatarFile.ContentType);
            avatarUrl = uploadedName ?? avatarUrl;
        }
        
        var updatedUser = await _repository.UpdateUserByEmail(
            id,
            email!,
            userName!,
            hashedPassword ?? user.PasswordHash!,
            avatarUrl!,
            ct
        );
        
        var token = _jwtProvider.GenerateToken(updatedUser!);

        return (updatedUser, token);
        
    }
}