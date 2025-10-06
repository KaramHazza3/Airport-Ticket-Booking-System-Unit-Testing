
using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common.Services;

public interface IBaseService<TEntity, TId>
{
    Task<Result<TEntity>> AddAsync(TEntity data);
}