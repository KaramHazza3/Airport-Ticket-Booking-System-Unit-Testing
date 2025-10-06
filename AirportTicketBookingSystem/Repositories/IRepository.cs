namespace AirportTicketBookingSystem.Repositories;

public interface IRepository
{
    Task<ICollection<T>> ReadAsync<T>() where T : class;
    Task WriteAsync<T>(List<T> data) where T : class;
}