using AirportTicketBookingSystem.Common.Helpers;
using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.ResultErrors;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Repositories;

namespace AirportTicketBookingSystem.Services.FlightService;

public class FlightService : IFlightService<Guid>
{
    private readonly IRepository _repository;
    private readonly List<Flight> _flights = [];

    public FlightService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ICollection<Flight>>> GetAllFlightsAsync()
    {
        return await GetFlights();
    }

    public async Task<Result<Flight>> AddFlightAsync(Flight flight)
    {
        var flights = await GetFlights();
        var isExist = flights.Any(f => f.Id == flight.Id);
        if (isExist)
        {
            return FlightErrors.AlreadyExists;
        } 
        flights.Add(flight);
        await this._repository.WriteAsync(flights);
        _flights.Clear();
        _flights.AddRange(flights);
        return flight;
    }

    public async Task<Result> DeleteFlightAsync(Guid flightId)
    {
        var flights = await GetFlights();
        var flightToRemove = flights.SingleOrDefault(b => b.Id == flightId);
        if (flightToRemove == null)
        {
            return Result.Failure(FlightErrors.NotFound);
        }

        flights.Remove(flightToRemove);
        await this._repository.WriteAsync(flights);
        _flights.Clear();
        _flights.AddRange(flights);
        return Result.Success();
    }

    public async Task<Result<Flight>> ModifyFlightAsync(Guid id, Flight flight)
    {
        var flights = await GetFlights();
        var index = flights.FindIndex(item => item.Id.Equals(id));
        if (index == -1)
        {
            return FlightErrors.NotFound;
        }
        flights[index] = flight;
        await this._repository.WriteAsync(flights);
        _flights.Clear();
        _flights.AddRange(flights);
        return flight;
    }

    public async Task<Result<Flight>> GetFlightById(Guid id)
    {
        var flights = await GetFlights();
        var flight = flights.SingleOrDefault(f => f.Id == id);
        if (flight == null)
        {
            return FlightErrors.NotFound;
        }

        return flight;
    }

    public async Task<Result<List<Flight>>> SearchFlight(params Func<Flight, bool>[] match)
    {
        return FilterHelper.FilterAsync(await GetFlights(), match);
    }
    
    private async Task<List<Flight>> GetFlights()
    {
        if (_flights.Count > 0)
        {
            return _flights;
        }
        var flights = await this._repository.ReadAsync<Flight>();
        _flights.Clear();
        _flights.AddRange(flights);
        return flights.ToList();
    }

    public async Task<Result<Flight>> AddAsync(Flight data)
    {
        return await AddFlightAsync(data);
    }
}