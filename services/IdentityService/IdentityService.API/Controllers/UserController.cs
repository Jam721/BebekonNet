using System.IdentityModel.Tokens.Jwt;
using IdentityService.API.Contracts.Users;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Application.Services;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[Route("identity/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserRepository repository, 
        IUserService service, 
        ILogger<UserController> logger)
    {
        _repository = repository;
        _service = service;
        _logger = logger;
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        try
        {
            await _service.Register(request.UserName, request.Email, request.Password);
            _logger.LogInformation($"User {request.UserName} successfully registered");
        }
        catch (Exception ex)
        {
            _logger.LogError($"User {request.UserName} failed to register: {ex.Message}");
        }
        
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            var token = await _service.Login(request.Email, request.Password);
            Response.Cookies.Append("tasty", token);
            _logger.LogInformation($"User {request.Email} logged in");
            
            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError($"User {request.Email} failed to login: {ex.Message}");
            throw new Exception();
        }
    }

    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            var user = await GetCurrentUser();
            
            if(user == null)
                return NotFound("User not found");
            
            _logger.LogInformation($"User {user.Email} logged in");
            
            return Ok(new
            {
                user.UserName,
                user.Email
            });
            
        }
        catch (Exception e)
        {
            _logger.LogInformation($"User not detected: {e.Message}");
            throw new Exception();
        }
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        if (!ModelState.IsValid)
            throw new Exception();
        var user = await GetCurrentUser();
        Response.Cookies.Delete("tasty");

        if (user != null)
        {
            _logger.LogInformation($"User {user.Email} logged out");
        }
        else
        {
            _logger.LogWarning("Unknown user attempted logout");
        }
    
        return Ok();
    }
    
    private async Task<User?> GetCurrentUser()
    {
        try
        {
            var token = Request.Cookies["tasty"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token not found in cookies");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenRead = tokenHandler.ReadJwtToken(token);
            var email = tokenRead.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email claim not found in token");
                return null;
            }

            return await _repository.GetUserByEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting current user: {ex.Message}");
            return null;
        }
    }
}