using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Todo_api.Models;
using Todo_api.Services.Implementation;
using Xunit;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly Mock<UserManager<AplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<AplicationUser>> _signInManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public AuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<AplicationUser>>();
        var passwordHasherMock = new Mock<IPasswordHasher<AplicationUser>>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<AplicationUser>>();

        _userManagerMock = new Mock<UserManager<AplicationUser>>(
            userStoreMock.Object,
            null,
            passwordHasherMock.Object,
            null,
            null,
            null,
            null,
            null,
            null
        );

        _signInManagerMock = new Mock<SignInManager<AplicationUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            userClaimsPrincipalFactoryMock.Object,
            null,
            null,
            null,
            null
        );

        _configurationMock = new Mock<IConfiguration>();

        // 🔹 Clave corregida (mínimo 32 caracteres)
        _configurationMock.Setup(c => c["Jwt:key"]).Returns("MySuperSecretKeyThatIsLongEnough123!");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsToken()
    {
        var user = new AplicationUser { UserName = "testUser", Id = "1" };

        _userManagerMock.Setup(u => u.FindByNameAsync("testUser")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, "password")).ReturnsAsync(true);

        var token = await _authService.Login("testUser", "password");

        Assert.NotNull(token);
        Assert.True(token.Length > 10);
    }

    [Fact]
    public async Task Login_InvalidUser_ThrowsUnauthorizedAccessException()
    {
        _userManagerMock.Setup(u => u.FindByNameAsync("wrongUser")).ReturnsAsync((AplicationUser)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login("wrongUser", "password"));
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsToken()
    {
        var user = new AplicationUser { UserName = "newUser" };

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AplicationUser>(), "password"))
            .ReturnsAsync(IdentityResult.Success);

        var token = await _authService.Register("newUser", "password");

        Assert.NotNull(token);
        Assert.True(token.Length > 10);
    }

    [Fact]
    public async Task Register_FailedRegistration_ThrowsException()
    {
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AplicationUser>(), "password"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Registration failed" }));

        await Assert.ThrowsAsync<Exception>(() => _authService.Register("newUser", "password"));
    }
}
