using System.Threading.Tasks;
using IdentityService.API.Contracts.Users;
using IdentityService.API.Controllers;
using IdentityService.Application.Interfaces.Repository.User;
using IdentityService.Application.Services;
using IdentityService.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<UsersService> _usersServiceMock = new(MockBehavior.Strict, null, null);
    private readonly Mock<ILogger<UserController>> _loggerMock = new();

    private UserController CreateController()
    {
        var controller = new UserController(_userRepositoryMock.Object, _usersServiceMock.Object, _loggerMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenModelIsValid()
    {
        var controller = CreateController();
        var request = new RegisterUserRequest
        {
            UserName = "TestUser",
            Email = "test@example.com",
            Password = "password123"
        };

        _usersServiceMock.Setup(s => s.Register(request.UserName, request.Email, request.Password))
            .Returns(Task.CompletedTask);

        var result = await controller.Register(request);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        var controller = CreateController();
        var request = new LoginUserRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var fakeToken = "fake.jwt.token";

        _usersServiceMock.Setup(s => s.Login(request.Email, request.Password))
            .ReturnsAsync(fakeToken);

        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(fakeToken, okResult.Value);
    }
}
