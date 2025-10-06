using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.ResultErrors;

public class BookingErrors
{
    public static readonly Error AlreadyExists = new("Booking.AlreadyExists", "The booking is already exists");
    public static readonly Error NotFound = new("Booking.NotFound", "The booking doesn't exist");
    public static readonly Error NotValid = new("Booking.NotValid", "The Booking is not valid");
}