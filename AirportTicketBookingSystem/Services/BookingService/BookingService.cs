using AirportTicketBookingSystem.Common.Helpers;
using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Repositories;

namespace AirportTicketBookingSystem.Services.BookingService;

public class BookingService : IBookingService<Guid>
{
    private readonly IRepository _repository;
    private readonly List<Booking> _bookings = [];

    public BookingService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ICollection<Booking>>> GetAllBookingsAsync()
    {
        return await GetBookings();
    }

    public async Task<Result<Booking>> AddBookingAsync(Booking booking)
    {
        var bookings = await GetBookings();
        var isExist = bookings.Any(b => b.Passenger.Id == booking.Passenger.Id && b.Flight.Id == booking.Flight.Id);
        if (isExist)
        {
            return BookingErrors.AlreadyExists;
        }

        if (booking.BookingDate < DateTime.UtcNow)
        {
            return BookingErrors.NotValid;
        }
        
        bookings.Add(booking);
        await this._repository.WriteAsync(bookings);
        _bookings.Clear();
        _bookings.AddRange(bookings);
        return booking;
    }

    public async Task<Result> CancelBookingAsync(Guid bookingId)
    {
        var bookings = await GetBookings();
        var canceledBooking = bookings.SingleOrDefault(b => b.Id == bookingId);
        if (canceledBooking == null)
        {
            return Result.Failure(BookingErrors.NotFound);
        }
        bookings.Remove(canceledBooking);
        await this._repository.WriteAsync(bookings);
        _bookings.Clear();
        _bookings.AddRange(bookings);
        return Result.Success();
    }

    public async Task<Result<Booking>> ModifyBookingAsync(Guid id, Booking booking)
    {
        var bookings = await GetBookings();
        var isExist = bookings.Exists(b => b.Id == id);
        if (!isExist)
        {
            return BookingErrors.NotFound;
        }
        var index = bookings.FindIndex(item => item.Id.Equals(booking.Id));
        bookings[index] = booking;
        await this._repository.WriteAsync(bookings);
        _bookings.Clear();
        _bookings.AddRange(bookings);
        return booking;
    }

    public async Task<Result<List<Booking>>> FilterAsync(params Func<Booking, bool>[] match)
    {
        return FilterHelper.FilterAsync(await GetBookings(), match);
    }

    private async Task<List<Booking>> GetBookings()
    {
        if (_bookings.Count > 0)
        {
            return _bookings;
        }

        var bookings = await _repository.ReadAsync<Booking>();
        _bookings.Clear();
        _bookings.AddRange(bookings);

        return _bookings;
    }

    public async Task<Result<Booking>> AddAsync(Booking data)
    {
        return await AddBookingAsync(data);
    }
}