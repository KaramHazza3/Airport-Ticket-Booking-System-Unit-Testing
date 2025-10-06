using System.IO;
namespace AirportTicketBookingSystem.Wrappers.File;

public class FileWrapper : IFileWrapper
{
    public Task WriteAllTextAsync(string filePath, string content) =>
        System.IO.File.WriteAllTextAsync(filePath, content);

    public Task<string> ReadAllTextAsync(string filePath) => System.IO.File.ReadAllTextAsync(filePath);

    public bool Exists(string filePath) => System.IO.File.Exists(filePath);
}