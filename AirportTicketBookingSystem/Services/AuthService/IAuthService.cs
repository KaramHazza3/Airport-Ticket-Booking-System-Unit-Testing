using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.Enums;

namespace AirportTicketBookingSystem.Services.AuthService;

public interface IAuthService
{
    Task<Result> RegisterAsync(string name, string email, string password, UserRole role);
    Task<Result<User>> LoginAsync(string email, string password);
}