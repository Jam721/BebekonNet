using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityService.API.Contracts.Users;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(
    IUserRepository repository,
    IUserService service,
    ILogger<UserController> logger)
    : ControllerBase
{
    [HttpPost("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AllUsers(CancellationToken cancellationToken)
    {
        var users = await repository.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(
        [FromForm] UpdateUserRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = await service.GetCurrentUser(cancellationToken);
            if (currentUser == null)
                return Unauthorized();
            
            var user = await service.UpdateUser(
                currentUser.Id, request.Email, request.UserName,
                request.Password, request.AvatarFile,
                cancellationToken);

            return Ok(user.user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromForm] RegisterUserRequest request, 
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            await service.Register(request.UserName, request.Email, request.Password, request.AvatarFile, cancellationToken);
            logger.LogInformation($"User {request.UserName} successfully registered");
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError($"User {request.UserName} failed to register: {ex.Message}");
            return BadRequest();
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginUserRequest request, 
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            var token = await service.Login(request.Email, request.Password, cancellationToken);
            logger.LogInformation($"User {request.Email} logged in");
            
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            logger.LogError($"User {request.Email} failed to login: {ex.Message}");
            return BadRequest();
        }
    }

    [HttpGet("me")]
    [Authorize(Policy = PermissionsConst.Read)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            var user = await service.GetCurrentUser(cancellationToken);
            
            if(user == null)
                return NotFound("User not found");
            
            logger.LogInformation($"User {user.Email} accessed Me endpoint");
            
            return Ok(new
            {
                user.UserName,
                user.Email,
                user.Permisions,
                user.CreatedAt,
                user.AvatarUrl
            });
        }
        catch (Exception e)
        {
            logger.LogInformation($"User not detected: {e.Message}");
            return BadRequest();
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        var user = await service.GetCurrentUser(cancellationToken);

        if (user != null)
        {
            logger.LogInformation($"User {user.Email} logged out");
        }
        else
        {
            logger.LogWarning("Unknown user attempted logout");
        }
    
        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpPost("give-access")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GiveAuthorship(string email, CancellationToken token)
    {
        try
        {
            await repository.GiveAuthorshipAsync(email, token);
            var user = await repository.GetUserByEmail(email, token);
            return Ok(user);
        }
        catch (Exception e)
        {
            logger.LogError($"User {email} failed to give authorship: {e.Message}");
            throw;
        }
    }

    [HttpGet("role")]
    public IActionResult GetCurrentRole()
    {
        try
        {
            var token = service.GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Token not found in Authorization header");
                return NotFound("Token not found");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenRead = tokenHandler.ReadJwtToken(token);
            var role = tokenRead.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(role)) return Ok(role);
            logger.LogWarning("Role claim not found in token");
            return NotFound("Role claim not found");

        }
        catch (Exception ex)
        {
            logger.LogError($"Error getting current user: {ex.Message}");
            return BadRequest();
        }
    }
}