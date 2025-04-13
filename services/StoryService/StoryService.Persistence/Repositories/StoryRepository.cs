using Microsoft.EntityFrameworkCore;
using StoryService.Application.Interfaces.Repositories;
using StoryService.Domain.Models;
using StoryService.Persistence.Entities;

namespace StoryService.Persistence.Repositories;

public class StoryRepository : IStoryRepository
{
    private readonly StoryDbContext _context;

    public StoryRepository(StoryDbContext context)
    {
        _context = context;
    }
    public async Task<List<StoryModel>?> GetAllStoryAsync(CancellationToken cancellationToken)
    {
        var stories = await _context.Stories
            .Include(st => st.Tags)
            .ToListAsync(cancellationToken);
        
        return stories.Select(s=>new StoryModel
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            Author = s.Author,
            StoryAvatarUrl = s.StoryAvatarUrl,
            Views = s.Views,
            Rating = s.Rating,
            IsNew = s.IsNew,
            Tags = s.Tags.Select(t=>new TagModel
            {
                Id = t.Id,
                Name = t.Name,
            }).ToList(),
        }).ToList();
    }

    public async Task<StoryModel?> GetStoryByIdAsync(Guid storyId, CancellationToken cancellationToken)
    {
        var s = await _context.Stories
            .Include(st=>st.Tags)
            .FirstOrDefaultAsync(st => st.Id == storyId, cancellationToken);
        if(s == null)
            return null;
        
        return new StoryModel
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            Author = s.Author,
            StoryAvatarUrl = s.StoryAvatarUrl,
            Views = s.Views,
            Rating = s.Rating,
            IsNew = s.IsNew,
            Tags = s.Tags.Select(t => new TagModel()
            {
                Id = t.Id,
                Name = t.Name,
            }).ToList(),
        };
    }

    public async Task AddStoryAsync(
        string title, 
        string description, 
        Guid author,
        string storyAvatarUrl,
        List<TagModel> tags,
        CancellationToken cancellationToken)
    {
        var storyEntity = new StoryEntity
        {
            Id = new Guid(),
            Title = title,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Author = author,
            StoryAvatarUrl = storyAvatarUrl,
            Views = 0,
            Rating = 0,
            IsNew = true,
            Tags = tags.Select(t => new TagEntity()
            {
                Id = t.Id,
                Name = t.Name,
            }).ToList(),
        };
        
        await _context.Stories.AddAsync(storyEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateStoryAsync(StoryModel story, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStoryAsync(Guid storyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}