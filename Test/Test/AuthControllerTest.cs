using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Todo_api.Controllers;
using Todo_api.Dtos;
using Todo_api.Services.Abstraction;
using Xunit;

public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly Mock<IAuthService> _authServiceMock;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ValidRequest_ReturnsToken()
    {
        var loginDto = new LoginDto { Username = "testUser", Password = "password" };

        _authServiceMock.Setup(a => a.Login(loginDto.Username, loginDto.Password))
            .ReturnsAsync("mockToken");

        var result = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnValue = okResult.Value;
        var tokenProperty = returnValue.GetType().GetProperty("Token");
        Assert.NotNull(tokenProperty);

        var tokenValue = tokenProperty.GetValue(returnValue)?.ToString();
        Assert.Equal("mockToken", tokenValue);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginDto = new LoginDto { Username = "testUser", Password = "wrongPassword" };

        _authServiceMock.Setup(a => a.Login(loginDto.Username, loginDto.Password))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.Login(loginDto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid credentials", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsToken()
    {
        var registerDto = new RegisterDto { Username = "newUser", Password = "password" };

        _authServiceMock.Setup(a => a.Register(registerDto.Username, registerDto.Password))
            .ReturnsAsync("mockToken");

        var result = await _controller.Register(registerDto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var returnValue = okResult.Value;
        var tokenProperty = returnValue.GetType().GetProperty("Token");
        Assert.NotNull(tokenProperty);

        var tokenValue = tokenProperty.GetValue(returnValue)?.ToString();
        Assert.Equal("mockToken", tokenValue);
    }

    [Fact]
    public async Task Register_FailedRequest_ReturnsBadRequest()
    {
        var registerDto = new RegisterDto { Username = "newUser", Password = "password" };

        _authServiceMock.Setup(a => a.Register(registerDto.Username, registerDto.Password))
            .ThrowsAsync(new Exception("User registration failed"));

        var result = await _controller.Register(registerDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("User registration failed", badRequestResult.Value);
    }
}
