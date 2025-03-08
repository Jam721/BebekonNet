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
    
    public async Task AddUserAsync(User user)
    {
        var userEntity = new UserEntity()
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            UserName = user.UserName
        };

        await _context.User.AddAsync(userEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var userEntity = await _context.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (userEntity == null) return null;

        return new User
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            PasswordHash = userEntity.PasswordHash,
            UserName = userEntity.UserName
        };
    }
}