using System.Globalization;
using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.Services;
using AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;
using AirportTicketBookingSystem.Wrappers;
using CsvHelper;
using CsvHelper.Configuration;

namespace AirportTicketBookingSystem.Services.ImportFileService;

public class ImportCsvFileService : IFileImportService
{
    private readonly ICSVReader _csvReader;

    public ImportCsvFileService(ICSVReader csvReader)
    {
        _csvReader = csvReader;
    }
    public async Task<Result<List<TEntity>>> ImportFileAsync<TEntity, TDto, TId>(
        string filePath,
        Func<TDto, TEntity> mapper,
        IBaseService<TEntity, TId> baseService,
        Func<TDto, int, List<CsvValidationError>> validator)
        where TEntity : IEntity<TId>
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<List<TEntity>>.Failure(new Error("Csv.FilePath", "File path cannot be empty."));

        try
        {
            var (entities, validationErrors) = await _csvReader.Read(filePath, mapper, validator);
            return await HandleAddition(baseService, validationErrors, entities);
        }
        catch (Exception ex)
        {
            var error = new Error("Csv.ReadError", $"Failed to read or parse the CSV file: {ex.Message}");
            return error;
        }
    }
    
    private static async Task<Result<List<TEntity>>> HandleAddition<TEntity, TId>(IBaseService<TEntity, TId> service, List<CsvValidationError> validationErrors, List<TEntity> entities) where TEntity : IEntity<TId>
    {
        if (validationErrors.Any())
        {
            var combinedMessage = string.Join("; ", validationErrors.Select(e => $"Row {e.RowNumber} {e.PropertyName}: {e.ErrorMessage}"));
            var error = new Error("Csv.ValidationError", combinedMessage);
            return error;
        }

        foreach (var entity in entities)
        {
            var result = await service.AddAsync(entity);
            if (result.IsFailure)
            {
                return Result<List<TEntity>>.Failure(result.Error);
            }
        }

        return entities;
    }
}