using System.Text.Json;

namespace AirportTicketBookingSystem.Repositories;

public class FileRepository : IRepository
{
    static FileRepository()
    {
        RootPath = FindProjectPath();
    }
    private FileRepository() {}
    private static FileRepository? _instance;
    private static readonly JsonSerializerOptions Options = new(){ WriteIndented = true };

    public static FileRepository Instance => _instance ??= new FileRepository();

    private static readonly string RootPath;
    
    public async Task<ICollection<T>> ReadAsync<T>() where T : class
    {
        var collectionName = typeof(T).Name;
        var filePath = GetFilePath(collectionName);
        await CreateFileIfDoesNotExist(filePath);
        return await ReadAsCollectionFromFileAsync<T>(filePath);
    }

    public async Task WriteAsync<T>(List<T> data) where T : class
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        var collectionName = typeof(T).Name;
        var filePath = GetFilePath(collectionName);
        await CreateFileIfDoesNotExist(filePath);
        await WriteCollectionToFileAsync(data, filePath);
    }

    // public async Task DeleteAsync<T>(T[] data) where T : class
    // {
    //     var collectionName = typeof(T).Name;
    //     var filePath = GetFilePath(collectionName);
    //     var items = await GetFileContentAsList<T>(filePath);
    //     var index = items.FindIndex(item => item.Equals(data));
    //     items.RemoveAt(index);
    //     await WriteCollectionToFileAsync(items, filePath);
    // }
    //
    // public async Task<T> UpdateAsync<T>(T[] data) where T : class
    // {
    //     var collectionName = typeof(T).Name;
    //     var filePath = GetFilePath(collectionName);
    //     var items = await GetFileContentAsList<T>(filePath);
    //     var index = items.FindIndex(item => item.Equals(data));
    //     items[index] = data;
    //     await WriteCollectionToFileAsync(items, filePath);
    //     return data;
    // }

    private static async Task<List<T>> GetFileContentAsList<T>(string filePath)
    {
        var fileContent = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<T>>(fileContent) ?? [];
    }

    private static async Task WriteCollectionToFileAsync<T>(ICollection<T> items, string filePath)
    {
        var json = JsonSerializer.Serialize(items, Options);
        await File.WriteAllTextAsync(filePath, json);
    }
    private static async Task CreateFileIfDoesNotExist(string filePath)
    {
        if (File.Exists(filePath)) return;
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await File.WriteAllTextAsync(filePath, "[]");
    }
    private static string FindProjectPath()
    {
        var current = Directory.GetCurrentDirectory();
        while (current != null && !File.Exists(Path.Combine(current, "Program.cs")))
        {
            current = Directory.GetParent(current)?.FullName;
        }
        return current!;
    }

    private static string GetFilePath(string collectionName)
    {
        return Path.Combine(RootPath, "Data", $"{collectionName}.json");
    }

    private static async Task<ICollection<T>> ReadAsCollectionFromFileAsync<T>(string filePath)
    where T : class
    {
        var fileContent = await File.ReadAllTextAsync(filePath);
        var serializedContent = JsonSerializer.Deserialize<List<T>>(fileContent);
        return serializedContent ?? [];
    }
}