using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.Services;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services.UserService;

public interface IUserService<TId> : IBaseService<User, TId>
{
    Task<Result<ICollection<User>>> GetAllUsersAsync();
    Task<Result<User>> AddUserAsync(User user);
    Task<Result> DeleteUserAsync(TId userId);
    Task<Result<User>> ModifyUserAsync(TId id, User user);
}