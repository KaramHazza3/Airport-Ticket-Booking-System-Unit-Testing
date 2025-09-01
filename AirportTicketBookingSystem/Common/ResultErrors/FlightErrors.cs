using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.ResultErrors;

public class FlightErrors
{
    public static readonly Error AlreadyExists = new("Flight.AlreadyExists", "The flight is already exists");
    public static readonly Error NotFound = new("Flight.NotFound", "The Flight doesn't exist");
}