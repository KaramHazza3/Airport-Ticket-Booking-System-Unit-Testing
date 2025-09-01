using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.ResultErrors;

public static class UserErrors
{
    public static readonly Error NotFound = new("Users.NotFound", "The user doesn't exist");
    public static readonly Error AlreadyExists = new("Users.AlreadyExists", "There is a user with same email exists");
}