namespace StoryService.API.Contracts;

public class CreateStoryRequest
{
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();
    
    public string StoryAvatarUrl { get; set; } = string.Empty;
}