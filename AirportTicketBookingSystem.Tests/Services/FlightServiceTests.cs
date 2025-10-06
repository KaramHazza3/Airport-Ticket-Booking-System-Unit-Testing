using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Repositories;
using AirportTicketBookingSystem.Services.FlightService;
using AirportTicketBookingSystem.Tests.Helpers;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class FlightServiceTests
{
    private readonly Mock<IRepository> _repositoryMock;
    private readonly IFlightService<Guid> _flightService;
    private readonly TestDataBuilder _builder;

    public FlightServiceTests()
    {
        var fixture = new Fixture();
        _repositoryMock = new Mock<IRepository>();
        _flightService = new FlightService(_repositoryMock.Object);
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight>());
        _builder = new TestDataBuilder(fixture);
    }

    [Fact]
    public async Task GetAllFlightsAsync_ShouldReturnSuccessfulResult()
    {
        var flight = _builder.BuildFlight();
        var flights = new List<Flight> { flight };
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(flights);

        var result = await _flightService.GetAllFlightsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(flights, result.Value);
    }

    [Fact]
    public async Task AddFlightAsync_ShouldReturnSuccessfulResult_WhenFlightDoesNotExist()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight>());

        var result = await _flightService.AddFlightAsync(flight);

        Assert.True(result.IsSuccess);
        Assert.Equal(flight, result.Value);
    }

    [Fact]
    public async Task AddFlightAsync_ShouldReturnFailureResult_WhenFlightExists()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight> { flight });

        var result = await _flightService.AddFlightAsync(flight);

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.AlreadyExists);
    }

    [Fact]
    public async Task AddFlightAsync_ShouldReturnFailureResult_WhenFlightDepartureDateIsNotInFuture()
    {
        var flight = _builder.BuildFlight(daysFromNow: -2);
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight>());

        var result = await _flightService.AddFlightAsync(flight);

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotValid);
    }

    [Fact]
    public async Task DeleteFlightAsync_ShouldReturnSuccessfulResult_WhenFlightExists()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight> { flight });

        var result = await _flightService.DeleteFlightAsync(flight.Id);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteFlightAsync_ShouldReturnFailureResult_WhenFlightDoesNotExists()
    {
        var result = await _flightService.DeleteFlightAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotFound);
    }

    [Fact]
    public async Task ModifyFlightAsync_ShouldReturnSuccessfulResult_WhenFlightExists()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight> { flight });
        var modifiedFlight = _builder.BuildFlight(basePrice: 200m);

        var result = await _flightService.ModifyFlightAsync(flight.Id, modifiedFlight);

        Assert.True(result.IsSuccess);
        Assert.Equal(200m, result.Value.BasePrice);
        Assert.Equal(modifiedFlight, result.Value);
    }

    [Fact]
    public async Task ModifyFlightAsync_ShouldReturnFailureResult_WhenFlightDoesNotExists()
    {
        var flight = _builder.BuildFlight();

        var result = await _flightService.ModifyFlightAsync(Guid.NewGuid(), flight);

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotFound);
    }

    [Fact]
    public async Task GetFlightById_ShouldReturnSuccessfulResult_WhenFlightIdExists()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight> { flight });

        var result = await _flightService.GetFlightById(flight.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(flight, result.Value);
    }

    [Fact]
    public async Task GetFlightById_ShouldReturnFailureResult_WhenFlightIdDoesNotExists()
    {
        var result = await _flightService.GetFlightById(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, FlightErrors.NotFound);
    }

    [Fact]
    public async Task SearchFlight_ShouldReturnSuccessfulResult_WhenFlightSearchExists()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight> { flight });

        var result = await _flightService.SearchFlight(f => f.BasePrice == 100m);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public async Task SearchFlight_ShouldReturnFailureResult_WhenFlightSearchDoesNotExists()
    {
        var flight = _builder.BuildFlight();
        _repositoryMock.Setup(x => x.ReadAsync<Flight>()).ReturnsAsync(new List<Flight> { flight });

        var result = await _flightService.SearchFlight(f => f.BasePrice == 200m);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task SearchFlight_ShouldThrowArgumentException_WhenFlightSearchContainsAnyNullPredictions()
    {
        Func<Flight, bool>?[] predicates = { f => f.BasePrice == 100m, null };

        await Assert.ThrowsAsync<ArgumentException>(async () => await _flightService.SearchFlight(predicates));
    }

    [Fact]
    public async Task SearchFlight_ShouldThrowArgumentException_WhenFlightSearchContainsNullPredictions()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _flightService.SearchFlight(null));
    }
}
