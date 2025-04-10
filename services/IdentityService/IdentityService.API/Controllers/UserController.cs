﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityService.API.Contracts.Users;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Models;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[Route("identity/[controller]")]
[ApiController]
public class UserController(
    IUserRepository repository,
    IUserService service,
    ILogger<UserController> logger)
    : ControllerBase
{
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        try
        {
            await service.Register(request.UserName, request.Email, request.Password);
            logger.LogInformation($"User {request.UserName} successfully registered");
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError($"User {request.UserName} failed to register: {ex.Message}");
            return BadRequest();
        }
        
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            var token = await service.Login(request.Email, request.Password);
            Response.Cookies.Append("tasty", token, new CookieOptions()
            {
                HttpOnly = true,
            });
            logger.LogInformation($"User {request.Email} logged in");
            
            return Ok(token);
        }
        catch (Exception ex)
        {
            logger.LogError($"User {request.Email} failed to login: {ex.Message}");
            return BadRequest();
        }
    }

    [HttpGet("Me")]
    [Authorize(Policy = Permissions.Read)]
    public async Task<IActionResult> Me()
    {
        if (!ModelState.IsValid)
            throw new Exception();
        
        try
        {
            var user = await GetCurrentUser();
            
            if(user == null)
                return NotFound("User not found");
            
            logger.LogInformation($"User {user.Email} logged in");
            
            return Ok(new
            {
                user.UserName,
                user.Email
            });
            
        }
        catch (Exception e)
        {
            logger.LogInformation($"User not detected: {e.Message}");
            return BadRequest();
        }
    }

    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (!ModelState.IsValid)
            throw new Exception();
        var user = await GetCurrentUser();
        Response.Cookies.Delete("tasty");

        if (user != null)
        {
            logger.LogInformation($"User {user.Email} logged out");
        }
        else
        {
            logger.LogWarning("Unknown user attempted logout");
        }
    
        return Ok();
    }

    [HttpGet("GetRole")]
    public IActionResult GetCurrentRole()
    {
        try
        {
            var token = Request.Cookies["tasty"];
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Token not found in cookies");
                return NotFound("Token not found");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenRead = tokenHandler.ReadJwtToken(token);
            var role = tokenRead.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
            {
                logger.LogWarning("Role claim not found in token");
                return NotFound("Role claim not found");
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error getting current user: {ex.Message}");
            return BadRequest();
        }
    }
    
    private async Task<UserModel?> GetCurrentUser()
    {
        try
        {
            var token = Request.Cookies["tasty"];
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Token not found in cookies");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenRead = tokenHandler.ReadJwtToken(token);
            var email = tokenRead.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                logger.LogWarning("Email claim not found in token");
                return null;
            }

            return await repository.GetUserByEmail(email);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error getting current user: {ex.Message}");
            return null;
        }
    }
}