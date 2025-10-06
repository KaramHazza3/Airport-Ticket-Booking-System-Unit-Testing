using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.Services;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services.BookingService;

public interface IBookingService<TId> : IBaseService<Booking, TId>, IFilterService<Booking>
{
  Task<Result<ICollection<Booking>>> GetAllBookingsAsync();
  Task<Result<Booking>> AddBookingAsync(Booking booking);
  Task<Result> CancelBookingAsync(TId bookingId);
  Task<Result<Booking>> ModifyBookingAsync(TId id, Booking booking);
}