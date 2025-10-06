using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.Enums;
using AirportTicketBookingSystem.Repositories;
using AirportTicketBookingSystem.Services.BookingService;
using AirportTicketBookingSystem.Tests.Helpers;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IRepository> _repositoryMock;
    private readonly IBookingService<Guid> _bookingService;
    private readonly TestDataBuilder _builder;

    public BookingServiceTests()
    {
        var fixture = new Fixture();
        _repositoryMock = new Mock<IRepository>();
        _bookingService = new BookingService(_repositoryMock.Object);
        _repositoryMock.Setup(x => x.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());
        _builder = new TestDataBuilder(fixture);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var booking = _builder.BuildBooking();
        var bookings = new List<Booking> { booking };
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(bookings, result.Value);
    }

    [Fact]
    public async Task AddBookingAsync_ShouldReturnSuccessfulResult_WhenBookingDoesNotExists()
    {
        // Arrange
        var booking = _builder.BuildBooking();
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.AddBookingAsync(booking);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(booking, result.Value);
    }

    [Fact]
    public async Task AddBookingAsync_ShouldReturnFailureResult_WhenSameBookingExists()
    {
        // Arrange
        var booking = _builder.BuildBooking();
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking> { booking });

        // Act
        var result = await _bookingService.AddBookingAsync(booking);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.AlreadyExists);
    }

    [Fact]
    public async Task AddBookingAsync_ShouldReturnFailureResult_WhenBookingDateIsNotInFuture()
    {
        // Arrange
        var booking = _builder.BuildBooking(bookingDate: DateTime.UtcNow.AddDays(-2));
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.AddBookingAsync(booking);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.NotValid);
    }

    [Fact]
    public async Task CancelBookingAsync_ShouldReturnSuccessfulResult_WhenBookingExists()
    {
        // Arrange
        var booking = _builder.BuildBooking();
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking> { booking });

        // Act
        var result = await _bookingService.CancelBookingAsync(booking.Id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CancelBookingAsync_ShouldReturnFailureResult_WhenBookingDoesNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.CancelBookingAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.NotFound);
    }

    [Fact]
    public async Task ModifyBookingAsync_ShouldReturnSuccessfulResult_WhenBookingExists()
    {
        // Arrange
        var booking = _builder.BuildBooking();
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking> { booking });
        var modifiedBooking = _builder.BuildBooking(price: 500m);

        // Act
        var result = await _bookingService.ModifyBookingAsync(booking.Id, modifiedBooking);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(modifiedBooking, result.Value);
    }

    [Fact]
    public async Task ModifyBookingAsync_ShouldReturnFailureResult_WhenBookingDoesNotExists()
    {
        // Arrange
        var booking = _builder.BuildBooking(price: 500m);
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.ModifyBookingAsync(Guid.NewGuid(), booking);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.NotFound);
    }

    [Fact]
    public async Task FilterBooking_ShouldReturnSuccessfulResult_WhenTheFilterExists()
    {
        // Arrange
        var booking = _builder.BuildBooking();
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking> { booking });

        // Act
        var result = await _bookingService.FilterAsync(b => b.Price == 100m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public async Task FilterBooking_ShouldReturnFailureResult_WhenFilterBookingDoesNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.FilterAsync(b => b.Price == 200m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task FilterBooking_ShouldThrowArgumentException_WhenFilterBookingContainsAnyNullPredictions()
    {
        // Arrange
        Func<Booking, bool>?[] predicates = { b => b.Price == 100m, null };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.FilterAsync(predicates));
    }

    [Fact]
    public async Task FilterBooking_ShouldThrowArgumentException_WhenFilterBookingContainsNullPredictions()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _bookingService.FilterAsync(null));
    }

}
