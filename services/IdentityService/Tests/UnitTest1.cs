using IdentityService.API.Controllers;
using IdentityService.API.Contracts.Users;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserService> _usersServiceMock = new();
    private readonly Mock<ILogger<UserController>> _loggerMock = new();

    private UserController CreateController()
    {
        var controller = new UserController(
            _userRepositoryMock.Object,
            _usersServiceMock.Object,
            _loggerMock.Object
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        var controller = CreateController();
        var request = new RegisterUserRequest
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "pass"
        };

        _usersServiceMock
            .Setup(x => x.Register(request.UserName, request.Email, request.Password))
            .Returns(Task.CompletedTask);
        
        var result = await controller.Register(request);
        
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithToken()
    {
        var controller = CreateController();
        var request = new LoginUserRequest
        {
            Email = "test@example.com",
            Password = "pass"
        };

        _usersServiceMock
            .Setup(x => x.Login(request.Email, request.Password))
            .ReturnsAsync("mocked.token.value");
        
        var result = await controller.Login(request);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("mocked.token.value", okResult.Value);
    }
}
