using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.Services;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services.FlightService;

public interface IFlightService<TId> : IBaseService<Flight, TId>
{
    Task<Result<ICollection<Flight>>> GetAllFlightsAsync();
    Task<Result<Flight>> AddFlightAsync(Flight flight);
    Task<Result> DeleteFlightAsync(TId flightId);
    Task<Result<Flight>> ModifyFlightAsync(TId id, Flight flight);
    Task<Result<Flight>> GetFlightById(TId id);
    Task<Result<List<Flight>>> SearchFlight(params Func<Flight, bool>[] match);
}