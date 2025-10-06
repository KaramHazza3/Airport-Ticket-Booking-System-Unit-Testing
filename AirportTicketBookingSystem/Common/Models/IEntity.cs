namespace AirportTicketBookingSystem.Common.Models;

public interface IEntity<T>
{
    T Id { get; init; }
}