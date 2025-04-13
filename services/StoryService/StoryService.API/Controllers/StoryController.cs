using Microsoft.AspNetCore.Mvc;
using StoryService.API.Contracts;
using StoryService.Application.Interfaces.Repositories;
using StoryService.Domain.Models;

namespace StoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoryController : ControllerBase
{
    private readonly ILogger<StoryController> _logger;
    private readonly IStoryRepository _repository;

    public StoryController(ILogger<StoryController> logger, IStoryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet("AllStories")]
    public async Task<IActionResult> GetAllStories(CancellationToken cancellationToken)
    {
        var stories = await _repository.GetAllStoryAsync(cancellationToken);
        
        return Ok(stories);
    }

    [HttpGet("StoryById")]
    public async Task<IActionResult> GetStoryById(Guid id, CancellationToken cancellationToken)
    {
        var story = await _repository.GetStoryByIdAsync(id, cancellationToken);
        
        return Ok(story);
    }

    [HttpPost("CreateStory")]
    public async Task<IActionResult> CreateStory(CreateStoryRequest request, CancellationToken cancellationToken)
    {
        
        var story = await _repository.AddStoryAsync(
            request.Title,
            request.Description,
            new Guid(),
            request.StoryAvatarUrl,
            request.Tags,
            cancellationToken);
        
        return Ok(story);
    }
}