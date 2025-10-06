using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Repositories;
using AirportTicketBookingSystem.Services.BookingService;
using AirportTicketBookingSystem.Tests.Customizations;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class BookingServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository> _repositoryMock;
    private readonly IBookingService<Guid> _bookingService;
    private readonly Booking _booking;
    private readonly List<Booking> _bookings;
    
    public BookingServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new FlightCustomization());
        _fixture.Customize(new BookingCustomization());
        _repositoryMock = new Mock<IRepository>();
        _bookingService = new BookingService(_repositoryMock.Object);
        _booking = _fixture.Create<Booking>();
        _bookings = new List<Booking>()
        {
            _booking
        };
        _repositoryMock.Setup(x => x.ReadAsync<Booking>()).ReturnsAsync(_bookings);
        _repositoryMock.Setup(x => x.WriteAsync(It.IsAny<List<Booking>>())).Returns(Task.CompletedTask);
    }
    
    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnSuccessfulResult()
    {
        // Arrange 
        
        // Act
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _bookings);
    }
    
    [Fact]
    public async Task AddBookingAsync_WhenBookingDoesNotExists_ShouldReturnSuccessfulResult()
    {
        // Arrange 
        _repositoryMock.Setup(x => x.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());
        
        // Act
        var result = await _bookingService.AddBookingAsync(_booking);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, _booking);
    }
    
    [Fact]
    public async Task AddBookingAsync_WhenSameBookingExists_ShouldReturnFailureResult()
    {
        // Arrange 
        
        // Act
        var result = await _bookingService.AddBookingAsync(_booking);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.AlreadyExists);
    }
    
    [Fact]
    public async Task AddBookingAsync_WhenBookingDateIsNotInFuture_ShouldReturnFailureResult()
    {
        // Arrange 
        _repositoryMock.Setup(x => x.ReadAsync<Booking>()).ReturnsAsync(new List<Booking>());
        _booking.BookingDate = DateTime.UtcNow.AddDays(-2);
        
        // Act
        var result = await _bookingService.AddBookingAsync(_booking);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.NotValid);
    }
    
    [Fact]
    public async Task CancelBookingAsync_WhenBookingExists_ShouldReturnSuccessfulResult()
    {
        // Arrange 
        
        // Act
        var result = await _bookingService.CancelBookingAsync(_booking.Id);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task CancelBookingAsync_WhenBookingDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange 
        
        // Act
        var result = await _bookingService.CancelBookingAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.NotFound);
    }
    
    [Fact]
    public async Task ModifyBookingAsync_WhenBookingExists_ShouldReturnSuccessfulResult()
    {
        // Arrange 
        var newBooking = _booking;
        newBooking.Price = 500m;
        // Act
        var result = await _bookingService.ModifyBookingAsync(_booking.Id, newBooking);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value, newBooking);
    }
    
    [Fact]
    public async Task ModifyBookingAsync_WhenBookingDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange 
        var newBooking = _booking;
        newBooking.Price = 500m;
        // Act
        var result = await _bookingService.ModifyBookingAsync(Guid.NewGuid(), newBooking);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, BookingErrors.NotFound);
    }
    
    [Fact]
    public async Task FilterBooking_WhenTheFilterExists_ShouldReturnSuccessfulResult()
    {
        // Arrange

        // Act
        var result = await _bookingService.FilterAsync(b => b.Price == 100m);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
    }
    
    [Fact]
    public async Task FilterBooking_WhenFilterBookingDoesNotExists_ShouldReturnFailureResult()
    {
        // Arrange

        // Act
        var result = await _bookingService.FilterAsync(b => b.Price == 200m);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
    
    [Fact]
    public async Task FilterBooking_WhenFilterBookingContainsAnyNullPredictions_ShouldThrowArgumentException()
    {
        // Arrange
        Func<Booking, bool>?[] predicates = { f => f.Price == 100m, null };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _bookingService.FilterAsync(predicates));
    }
    
    [Fact]
    public async Task FilterBooking_WhenFilterBookingContainsNullPredictions_ShouldThrowArgumentException()
    {
        // Arrange
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _bookingService.FilterAsync(null));
    }
}