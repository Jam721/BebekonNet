using System.IdentityModel.Tokens.Jwt;
using IdentityService.API.Contracts.Users;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[Route("identity/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly UsersService _service;

    public UserController(IUserRepository repository, UsersService service)
    {
        _repository = repository;
        _service = service;
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        await _service.Register(request.UserName, request.Email, request.Password);
        
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var token = await _service.Login(request.Email, request.Password);
        
        Response.Cookies.Append("tasty", token);
        
        return Ok(token);
    }

    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var token = Request.Cookies["tasty"];
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenRead = tokenHandler.ReadJwtToken(token);
        var email = tokenRead.Claims.FirstOrDefault(c => c.Type == "email")!.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email not found in token.");
        }

        var user = await _repository.GetUserByEmail(email);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(new
        {
            UserName = user.UserName,
            Email = user.Email
        });
    }
}