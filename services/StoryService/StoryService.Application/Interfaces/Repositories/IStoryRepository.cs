using StoryService.Domain.Models;

namespace StoryService.Application.Interfaces.Repositories;

public interface IStoryRepository
{
    Task<List<StoryModel>?> GetAllStoryAsync(CancellationToken cancellationToken = default);
    Task<StoryModel?> GetStoryByIdAsync(Guid storyId, CancellationToken cancellationToken = default);

    Task AddStoryAsync(
        string title,
        string description,
        Guid author,
        string storyAvatarUrl,
        List<TagModel> tags,
        CancellationToken cancellationToken);
    Task UpdateStoryAsync(StoryModel story, CancellationToken cancellationToken = default);
    Task DeleteStoryAsync(Guid storyId, CancellationToken cancellationToken = default);
}