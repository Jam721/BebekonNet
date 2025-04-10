using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Domain.Models;
using IdentitySevice.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentitySevice.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }
    
    public async Task AddUserAsync(UserModel userModel)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == userModel.Email);
    
        if (emailExists)
        {
            throw new InvalidOperationException($"Email {userModel.Email} already registered");
        }
    
        var readPermission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == "Read");
    
        if (readPermission == null)
        {
            throw new InvalidOperationException("Read permission not found in database");
        }

        var userEntity = new UserEntity
        {
            Id = userModel.Id,
            Email = userModel.Email,
            PasswordHash = userModel.PasswordHash,
            UserName = userModel.UserName,
            Permissions = new List<PermissionEntity> { readPermission }
        };
    
        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<UserModel?> GetUserByEmail(string email)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (userEntity == null) return null;

        return new UserModel
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            PasswordHash = userEntity.PasswordHash,
            UserName = userEntity.UserName
        };
    }

    public async Task<ICollection<PermissionModel>> GetPermissionByEmail(string email)
    {
        var user = await _context.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Email == email);

        return user?.Permissions
            .Select(p => new PermissionModel { Name = p.Name })
            .ToList() ?? new List<PermissionModel>();
    }
}