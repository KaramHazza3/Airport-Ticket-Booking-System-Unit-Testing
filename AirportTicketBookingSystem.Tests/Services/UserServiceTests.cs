using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Repositories;
using AirportTicketBookingSystem.Services.UserService;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IRepository> _repositoryMock;
    private readonly User _user;
    private readonly IUserService<Guid> _userService;
    private readonly List<User> _users;
        
    public UserServiceTests()
    {
        var fixture = new Fixture();
        _repositoryMock = new Mock<IRepository>();
        _userService = new UserService(_repositoryMock.Object);
        _user = fixture.Create<User>();
        _users = new List<User>() { _user };
        _repositoryMock.Setup(x => x.ReadAsync<User>()).ReturnsAsync(_users);
        _repositoryMock.Setup(x => x.WriteAsync(It.IsAny<List<User>>())).Returns(Task.CompletedTask);
    }
    
    [Fact]
    public async Task GetAllUsersAsync_WhenThereIsUsers_ShouldReturnSuccessfulResult()
    {
        // Arrange
    
        
        // Act
        var result = await _userService.GetAllUsersAsync();
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_users, result.Value);
        _repositoryMock.Verify(x => x.ReadAsync<User>(), Times.Once);
    }
    
    [Fact]
    public async Task AddUserAsync_WhenUserDoesNotExist_ShouldReturnSuccessfulResult()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ReadAsync<User>()).ReturnsAsync(new List<User>());
        
        // Act
        var result = await _userService.AddUserAsync(_user);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_user, result.Value);
        _repositoryMock.Verify(x => x.WriteAsync(It.IsAny<List<User>>()), Times.Once);
    }
    
    [Fact]
    public async Task AddUserAsync_WhenSameUserExists_ShouldReturnFailureResult()
    {
        // Arrange
        
        // Act
        var result = await _userService.AddUserAsync(_user);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, UserErrors.AlreadyExists);
        _repositoryMock.Verify(x => x.WriteAsync(It.IsAny<List<User>>()), Times.Never);
    }

}