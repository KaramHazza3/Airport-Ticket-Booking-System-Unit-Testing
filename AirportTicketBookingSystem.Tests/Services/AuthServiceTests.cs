using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Services.AuthService;
using AirportTicketBookingSystem.Services.UserService;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class AuthServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserService<Guid>> _userServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _fixture = new Fixture();
        _userServiceMock = new Mock<IUserService<Guid>>();
        _authService = new AuthService(_userServiceMock.Object);
        _userServiceMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .ReturnsAsync(Result<User>.Success(_fixture.Create<User>()));
    }
    
    [Fact]
    public async Task RegisterAsync_WhenUserDoesNotExists_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        
        // Act
        var result = await _authService.RegisterAsync(user.Name, user.Email, user.Password, user.Role);
        
        // Assert
        Assert.True(result.IsSuccess);
        _userServiceMock.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);
    }
    
    [Fact]
    public async Task RegisterAsync_WhenUserAlreadyExists_ShouldReturnFailureResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userServiceMock.Setup(x => x.AddUserAsync(It.IsAny<User>()))
                        .ReturnsAsync(Result<User>.Failure(UserErrors.AlreadyExists));
        
        // Act
        var result = await _authService.RegisterAsync(user.Name, user.Email, user.Password, user.Role);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadyExists, result.Error);
        _userServiceMock.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);
    }
    
    [Fact]
    public async Task LoginAsync_WhenUserExists_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var users = new List<User>();
        users.Add(user);
        
        _userServiceMock.Setup(x => x.GetAllUsersAsync())
                        .ReturnsAsync(users);
        
        // Act
        var result = await _authService.LoginAsync(user.Email, user.Password);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user, result.Value);
        _userServiceMock.Verify(x => x.GetAllUsersAsync(), Times.Once);

    }
    
    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var users = new List<User>();
        
        _userServiceMock.Setup(x => x.GetAllUsersAsync())
                        .ReturnsAsync(users);
        
        // Act
        var result = await _authService.LoginAsync(user.Email, user.Password);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(AuthErrors.Unauthorized, result.Error);
        _userServiceMock.Verify(x => x.GetAllUsersAsync(), Times.Once);
    }
    
}