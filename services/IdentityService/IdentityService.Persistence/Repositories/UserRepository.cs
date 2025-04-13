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

    public async Task<List<UserModel>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _context.Users
            .Include(userEntity => userEntity.Permissions)
            .ToListAsync(ct);

        return users.Select(u=>new UserModel()
        {
            Id = u.Id,
            UserName = u.UserName,
            PasswordHash = u.PasswordHash,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            AvatarUrl = u.AvatarUrl,
            Permisions = u.Permissions.Select(p=>new PermissionModel()
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList(),
        }).ToList();
    }

    public async Task AddUserAsync(UserModel userModel, CancellationToken cancellationToken = default)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == userModel.Email, cancellationToken);
    
        if (emailExists)
        {
            throw new InvalidOperationException($"Email {userModel.Email} already registered");
        }
    
        var readPermission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == "Read", cancellationToken);
    
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
            CreatedAt = DateTime.UtcNow,
            AvatarUrl = userModel.AvatarUrl,
            Permissions = new List<PermissionEntity> { readPermission }
        };
    
        await _context.Users.AddAsync(userEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserModel?> UpdateUserByEmail(
        Guid id,
        string? email,
        string? userName,
        string? passwordHash,
        string? avatarUrl,
        CancellationToken ct = default)
    {
        var userEntity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (userEntity == null)
            return null;
        
        if (!string.IsNullOrEmpty(email) && email != userEntity.Email)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == email, ct);
            if (emailExists)
                throw new InvalidOperationException("Email already exists");
        }
        
        userEntity.Email = email ?? userEntity.Email;
        userEntity.UserName = userName ?? userEntity.UserName;
        userEntity.PasswordHash = passwordHash ?? userEntity.PasswordHash;
        userEntity.AvatarUrl = avatarUrl ?? userEntity.AvatarUrl;

        await _context.SaveChangesAsync(ct);

        return new UserModel()
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            UserName = userEntity.UserName,
            PasswordHash = userEntity.PasswordHash,
            AvatarUrl = userEntity.AvatarUrl
        };
    }


    public async Task<UserModel?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users
            .Include(userEntity => userEntity.Permissions)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (userEntity == null) return null;

        return new UserModel
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            PasswordHash = userEntity.PasswordHash,
            UserName = userEntity.UserName,
            CreatedAt = userEntity.CreatedAt,
            AvatarUrl = userEntity.AvatarUrl,
            Permisions = userEntity.Permissions.Select(p => new PermissionModel()
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList()
        };
    }

    public async Task<UserModel?> GetUserById(Guid id, CancellationToken ct = default)
    {
        var userEntity = await _context.Users
            .Include(userEntity => userEntity.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
        
        if (userEntity == null) return null;
        
        return new UserModel
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            PasswordHash = userEntity.PasswordHash,
            UserName = userEntity.UserName,
            CreatedAt = userEntity.CreatedAt,
            AvatarUrl = userEntity.AvatarUrl,
            Permisions = userEntity.Permissions.Select(p => new PermissionModel()
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList()
        };
    }

    public async Task<ICollection<PermissionModel>> GetPermissionByEmail(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return user?.Permissions
            .Select(p => new PermissionModel { Name = p.Name })
            .ToList() ?? new List<PermissionModel>();
    }

    public async Task GiveAuthorshipAsync(string email, CancellationToken ct = default)
    {
        var permissionCreate = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == "Create", ct);
        var permissionUpdate = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == "Update", ct);
        var permissionDelete = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == "Delete", ct);
        var permissionRead = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == "Read", ct);
        
        var user = await _context.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user == null) 
            throw new ArgumentException("User not found");
        
        var permissionsToAdd = new[] { permissionCreate!, permissionUpdate!, permissionDelete!, permissionRead! }
            .Where(p => !user.Permissions.Any(up => up.Id == p.Id))
            .ToList();

        user.Permissions.AddRange(permissionsToAdd);
        await _context.SaveChangesAsync(ct);
    }
    
    
}