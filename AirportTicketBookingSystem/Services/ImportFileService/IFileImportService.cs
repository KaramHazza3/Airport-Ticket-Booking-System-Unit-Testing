using AirportTicketBookingSystem.Common.Models;
using AirportTicketBookingSystem.Common.Services;
using AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;

namespace AirportTicketBookingSystem.Services.ImportFileService;

public interface IFileImportService
{
    Task<Result<List<TEntity>>> ImportFileAsync<TEntity, TDto, TId>(
        string filePath,
        Func<TDto, TEntity> mapper,
        IBaseService<TEntity, TId> baseService,
        Func<TDto, int, List<CsvValidationError>> validator)
        where TEntity : IEntity<TId>;
}