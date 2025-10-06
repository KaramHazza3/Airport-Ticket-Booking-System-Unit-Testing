using AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;
using CsvHelper;

namespace AirportTicketBookingSystem.Wrappers;

public interface ICSVReader
{
    public Task<(List<TEntity>,List<CsvValidationError>)> Read<TEntity, TDto>(string filePath,
        Func<TDto, TEntity> mapper,
        Func<TDto, int, List<CsvValidationError>> validator);
}