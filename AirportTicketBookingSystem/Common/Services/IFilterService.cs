using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.Services;

public interface IFilterService<T>
{
    Task<Result<List<T>>> FilterAsync(params Func<T, bool>[] match);
}