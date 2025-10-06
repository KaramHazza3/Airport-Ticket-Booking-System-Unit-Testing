namespace AirportTicketBookingSystem.Wrappers.File;

public interface IFileWrapper
{
    public Task WriteAllTextAsync(string filePath, string content);
    public Task<string> ReadAllTextAsync(string filePath);
    public bool Exists(string filePath);
}