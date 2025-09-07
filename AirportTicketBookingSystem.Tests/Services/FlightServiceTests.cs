using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.Enums;
using AirportTicketBookingSystem.Repositories;
using AirportTicketBookingSystem.Services.FlightService;
using AirportTicketBookingSystem.Tests.Customizations;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class FlightServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository> _repositoryMock;
    private readonly IFlightService<Guid> _flightService;
    private readonly Flight _flight;
    private readonly List<Flight> _flights;
    
    public FlightServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new FlightCustomization());
        _flight = _fixture.Create<Flight>();
        _flights = new List<Flight>()
        {
            _flight
        };
        _repositoryMock = new Mock<IRepository>();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(_flights);
        _repositoryMock.Setup(x => x.WriteAsync(It.IsAny<List<Flight>>())).Returns(Task.CompletedTask);
        _flightService = new FlightService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllFlightsAsync_ShouldReturnSuccessfulResult()
    {
        // Arrange
        
        // Act
        var result = await _flightService.GetAllFlightsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _flights);
    }
    
    [Fact]
    public async Task AddFlightAsync_WhenFlightDoesNotExist_ShouldReturnSuccessfulResult()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight>());

        // Act
        var result = await _flightService.AddFlightAsync(_flight);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _flight);
    }
    
    [Fact]
    public async Task AddFlightAsync_WhenFlightExists_ShouldReturnFailureResult()
    {
        // Arrange

        // Act
        var result = await _flightService.AddFlightAsync(_flight);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.AlreadyExists);
    }
    
    [Fact]
    public async Task AddFlightAsync_WhenFlightDepartureDateIsNotInFuture_ShouldReturnFailureResult()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight>());

        _flight.DepartureDate = DateTime.UtcNow.AddDays(-2);
        
        // Act
        var result = await _flightService.AddFlightAsync(_flight);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotValid);
    }
    
    [Fact]
    public async Task DeleteFlightAsync_WhenFlightExists_ShouldReturnSuccessfulResult()
    {
        // Arrange

        // Act
        var result = await _flightService.DeleteFlightAsync(_flight.Id);
        
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task DeleteFlightAsync_WhenFlightDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange

        // Act
        var result = await _flightService.DeleteFlightAsync(Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotFound);
    }
    
    [Fact]
    public async Task ModifyFlightAsync_WhenFlightExists_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var newFlight = _flight;
        newFlight.BasePrice = 200m;
        
        // Act
        var result = await _flightService.ModifyFlightAsync(_flight.Id, newFlight);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.Value.BasePrice);
        Assert.Equal(result.Value, _flight);
    }
    
    [Fact]
    public async Task ModifyFlightAsync_WhenFlightDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange

        // Act
        var result = await _flightService.ModifyFlightAsync(Guid.NewGuid(), _flight);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotFound);
    }
    
    [Fact]
    public async Task GetFlightById_WhenFlightIdExists_ShouldReturnSuccessfulResult()
    {
        // Arrange

        // Act
        var result = await _flightService.GetFlightById(_flight.Id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _flight);
    }
    
    [Fact]
    public async Task GetFlightById_WhenFlightIdDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange

        // Act
        var result = await _flightService.GetFlightById(Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotFound);
    }
    
    [Fact]
    public async Task SearchFlight_WhenFlightSearchExists_ShouldReturnSuccessfulResult()
    {
        // Arrange

        // Act
        var result = await _flightService.SearchFlight(f => f.BasePrice == 100m);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
    }
    
    [Fact]
    public async Task SearchFlight_WhenFlightSearchDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange

        // Act
        var result = await _flightService.SearchFlight(f => f.BasePrice == 200m);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
    
    [Fact]
    public async Task SearchFlight_WhenFlightSearchContainsAnyNullPredictions_ShouldThrowArgumentException()
    {
        // Arrange
        Func<Flight, bool>?[] predicates = { f => f.BasePrice == 100m, null };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _flightService.SearchFlight(predicates));
    }
    
    [Fact]
    public async Task SearchFlight_WhenFlightSearchContainsNullPredictions_ShouldThrowArgumentException()
    {
        // Arrange
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _flightService.SearchFlight(null));
    }
}