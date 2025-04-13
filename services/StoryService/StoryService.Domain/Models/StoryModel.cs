namespace StoryService.Domain.Models;

public class StoryModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<TagModel> Tags { get; set; } = new();
    
    public int Views { get; set; } = 0;
    
    public string StoryAvatarUrl { get; set; } = string.Empty;

    public Guid Author { get; set; }

    public double Rating { get; set; } = 0.0;

    public bool IsNew { get; set; } = true;
}