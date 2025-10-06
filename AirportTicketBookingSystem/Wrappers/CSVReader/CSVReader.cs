using System.Globalization;
using AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace AirportTicketBookingSystem.Wrappers;

public class CSVReader : ICSVReader
{
    public async Task<(List<TEntity>, List<CsvValidationError>)> Read<TEntity, TDto>(string filePath, Func<TDto, TEntity> mapper, Func<TDto, int, List<CsvValidationError>> validator)
    {
        var entities = new List<TEntity>();
        var validationErrors = new List<CsvValidationError>();
        var rowNumber = 2; 
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null
        });

        await foreach (var dto in csv.GetRecordsAsync<TDto>())
        {
            var errors = validator(dto, rowNumber);
            if (errors.Count == 0)
            {
                entities.Add(mapper(dto));
            }
            else
            {
                validationErrors.AddRange(errors);
            }
            
            rowNumber++;
        }

        return (entities, validationErrors);
    }
    
}