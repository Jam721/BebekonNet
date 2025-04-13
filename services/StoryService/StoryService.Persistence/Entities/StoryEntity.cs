namespace StoryService.Persistence.Entities;

public class StoryEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<TagEntity> Tags { get; set; } = new();

    public string StoryAvatarUrl { get; set; } = string.Empty;
    public Guid Author { get; set; }

    public int Views { get; set; } = 0;

    public double Rating { get; set; } = 0.0;

    public bool IsNew { get; set; } = true;
}