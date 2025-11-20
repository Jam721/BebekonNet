using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityService.Application.Interfaces.Auth;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Application.Services;

public class UsersService(
    IPasswordHasher passwordHasher,
    IUserRepository repository,
    IJwtProvider jwtProvider,
    IFileStorageService fileStorageService,
    IHttpContextAccessor httpContextAccessor)
    : IUserService
{
    public async Task Register(
        string userName, 
        string email, 
        string password, 
        IFormFile? avatar, 
        CancellationToken cancellationToken = default)
    {
        var hashedPassword = passwordHasher.Generate(password);
    
        var avatarObjectName = "default-avatar.png";

        if (avatar is { Length: > 0 })
        {
            await using var stream = avatar.OpenReadStream();
            var uploadedName = await fileStorageService.UploadAvatarAsync(
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

        await repository.AddUserAsync(user, cancellationToken);
    }

    public async Task<string> Login(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByEmail(email, cancellationToken);

        if (user == null) throw new Exception("Пользователь не найден");

        var result = passwordHasher.Verify(password, user.PasswordHash);

        if (result == false) throw new Exception("Failed to Login");

        var token = jwtProvider.GenerateToken(user);

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
        var user = await repository.GetUserById(id, ct);
        if (user == null)
            throw new ArgumentException("User not found");
        
        string? hashedPassword = null;
        if (!string.IsNullOrEmpty(password))
        {
            hashedPassword = passwordHasher.Generate(password);
        }
        
        var avatarUrl = user.AvatarUrl;
        if (avatarFile is { Length: > 0 })
        {
            if (avatarUrl != "default-avatar.png")
            {
                await fileStorageService.DeleteAvatarAsync(avatarUrl!);
            }
            
            await using var stream = avatarFile.OpenReadStream();
            var uploadedName = await fileStorageService.UploadAvatarAsync(
                stream,
                avatarFile.FileName,
                avatarFile.ContentType);
            avatarUrl = uploadedName ?? avatarUrl;
        }
        
        var updatedUser = await repository.UpdateUserByEmail(
            id,
            email!,
            userName!,
            hashedPassword ?? user.PasswordHash!,
            avatarUrl!,
            ct
        );
        
        var token = jwtProvider.GenerateToken(updatedUser!);

        return (updatedUser, token);
    }

    public async Task<UserModel?> GetCurrentUser(CancellationToken ct = default)
    {
        try
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenRead = tokenHandler.ReadJwtToken(token);
            var email = tokenRead.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            return await repository.GetUserByEmail(email, ct);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string? GetTokenFromHeader()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            return null;

        return authorizationHeader["Bearer ".Length..].Trim();
    }
}