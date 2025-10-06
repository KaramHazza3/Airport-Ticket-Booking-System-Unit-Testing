using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.ResultErrors;

public class AuthErrors
{
    public static readonly Error Unauthorized = new("Auth.Unauthorized", "Invalid credentials");
    public static readonly Error NotValid = new("Auth.NotValid", "The input is invalid, nothing should be empty or null");

}